using System.Text;
using AutoMapper.Internal;
using Common.Extensions;
using Common.Helpers;
using DinkToPdf.Contracts;
using HtmlAgilityPack;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.SalaryModels;
using ManageEmployee.Entities.InOutEntities;
using ManageEmployee.Entities.SalaryEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Generators;
using ManageEmployee.Services.Interfaces.Users.Salaries;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace ManageEmployee.Services.UserServices.SalaryServices;
public class CalculateSalaryService: ICalculateSalaryService
{
    private readonly ApplicationDbContext _context;
    private readonly IConverter _converterPDF;
    private readonly IPdfGeneratorService _pdfGeneratorService;

    public CalculateSalaryService(ApplicationDbContext context, IConverter converterPdf, IPdfGeneratorService pdfGeneratorService)
    {
        _context = context;
        _converterPDF = converterPdf;
        _pdfGeneratorService = pdfGeneratorService;
    }
    public async Task<PagingResult<SalaryReportModel>> GetPaging(PagingRequestFilterByMonthModel param)
    {
        var queryable = GetSalariesByMonthQueryable(param.Month, param.Year);
        var pagingResult = await queryable.Skip(param.PageSize * param.Page).Take(param.PageSize).ToListAsync();

        return new PagingResult<SalaryReportModel>
        {
            PageSize = param.PageSize,
            CurrentPage = param.Page,
            Data = pagingResult,
            TotalItems = await queryable.CountAsync()
        };
    }
    
    /// <summary>
    /// Prepare before calculate salary
    /// </summary>
    /// <param name="month"></param>
    /// <param name="year"></param>
    /// <returns></returns>
    private async Task<(List<InOutHistoryPeriodModel> histories, List<SalaryChangePeriodModel> salaryChangePeriods, List<ContractBasedSalaryModel> contractBasedSalaries, List<SalaryAllowanceModel> userAllowances, List<NumberOfMealDetail> mealDetails, List<UserCommissionModel> userCommissions)> PrepareCalculateData(int month, int year)
    {
        // Get history by month and year
        var histories = await _context.InOutHistories
            .Where(history => history.TimeIn != null &&
                              history.TimeOut != null &&
                              history.TimeFrameFrom != null &&
                              history.TimeFrameTo != null &&
                              history.TargetDate.Month == month &&
                              history.TargetDate.Year == year
            ).Select(history => new InOutHistoryPeriodModel
            {
                UserId = history.UserId,
                TimeIn = history.TimeIn.Value,
                TimeOut = history.TimeOut.Value,
                SymbolId = history.SymbolId,
                TimeFrameFrom = history.TimeFrameFrom,
                TimeFrameTo = history.TimeFrameTo,
                TargetDate = history.TargetDate
            }).ToListAsync();

        var salaryChangePeriods = await _context.SalaryUserVersions
            .Select(x => new SalaryChangePeriodModel
            {
                UserId = x.UserId,
                EffectiveFrom = x.EffectiveFrom,
                EffectiveTo = x.EffectiveTo ?? DateTime.Now,
                SalaryTo = x.SalaryTo,
                Percent = x.Percent,
            }).ToListAsync();

        var contractBasedSalaries = await (
            from detail in _context.SalaryTypeProduceProductDetails
            join p in _context.SalaryTypeProduceProducts on detail.SalaryTypeProduceProductId equals p.Id
            join salaryType in _context.SalaryTypes on p.SalaryTypeId equals salaryType.Id 
            where p.CreatedAt.Month == month &&
                  p.CreatedAt.Year == year
            select new ContractBasedSalaryModel
            {
                UserId = detail.UserId,
                Percent = detail.Percent,
                AmountSpentMonthly = salaryType.AmountSpentMonthly,
                Quantity = p.Quantity
            }).ToListAsync();

        var userAllowances = await _context.AllowanceUsers
            .Select(x => new SalaryAllowanceModel
            {
                UserId = x.UserId,
                WorkingDaysFrom = x.WorkingDaysFrom,
                WorkingDaysTo = x.WorkingDaysTo,
                Value = x.Value
            })
            .ToListAsync();

        var mealDetails = await _context.NumberOfMealDetails.Where(x =>
            x.Date.Month == month &&
            x.Date.Year == year &&
            x.QuantityAdd > 0
        ).ToListAsync();

        var userRevenues = await CollectBillCommission(month, year);
        
        return (histories, salaryChangePeriods, contractBasedSalaries, userAllowances, mealDetails, userRevenues);
    }

    public async Task CalculateSalaryByMonth(int month, int year)
    {
        // Prepare data from DB before calculate
        var (
            histories,
            salaryChangePeriods,
            contractBasedSalaries,
            userAllowances,
            mealDetails,
            userCommissions
            ) = await PrepareCalculateData(month, year);

        var historyGrouped = histories.GroupBy(x => new { x.UserId, x.NumberWorkdays }).ToList();

        var userSalaries = new List<Salary>();
        foreach (var historyByUser in historyGrouped)
        {
            var userId = historyByUser.Key.UserId;
            var userInOutHistories = historyByUser.ToList();
            var salaryResult = await CalculateByPeriodChanges(
                userId: userId,
                month: month,
                year: year,
                salaryChangePeriods: salaryChangePeriods,
                userInOutHistories: userInOutHistories,
                contractualSalaries: contractBasedSalaries,
                userAllowances: userAllowances,
                mealDetails: mealDetails,
                userCommissions: userCommissions
            );
            if (salaryResult != null)
            {
                userSalaries.Add(salaryResult);
            }
        }
        
        // Remove old data
        var removeList =  await _context.Salaries.Where(x => x.Month == month && x.Year == year).ToListAsync();
        
        _context.RemoveRange(removeList);
        // Add salaries calculated
        await _context.AddRangeAsync(userSalaries);
        await _context.SaveChangesAsync();
    }
    
    private async Task<Salary> CalculateByPeriodChanges(int userId,
        int month,
        int year,
        IEnumerable<SalaryChangePeriodModel> salaryChangePeriods,
        List<InOutHistoryPeriodModel> userInOutHistories,
        List<ContractBasedSalaryModel> contractualSalaries,
        List<SalaryAllowanceModel> userAllowances,
        List<NumberOfMealDetail> mealDetails, 
        List<UserCommissionModel> userCommissions)
    {
        var daysNumberInMonth = DateTime.DaysInMonth(year, month);
        var fromDate = new DateTime(year, month, 1);
        var toDate = new DateTime(year, month, day: daysNumberInMonth);
        
        var overtimes = await _context.ProcedureRequestOvertimes
            .Where(x =>
                x.FromAt.Date >= fromDate.Date &&
                x.ToAt.Date <= toDate.Date &&
                x.UserIdStr == userId.ToString()
            ).ToListAsync();
        
        // Change periods by user
        var changePeriods = salaryChangePeriods
            .Where(x =>
                x.UserId == userId &&
                (
                    fromDate.IsDateBetween(x.EffectiveFrom, x.EffectiveTo) ||
                    toDate.IsDateBetween(x.EffectiveFrom, x.EffectiveTo)
                ))
            .OrderBy(x => x.EffectiveFrom)
            .Select(x => new
            {
                x.EstimatedSalary,
                FromDate = CommonHelper.GetMaxValue(fromDate, x.EffectiveFrom),
                ToDate = CommonHelper.GetMinValue(toDate, x.EffectiveTo),
            }).ToList();

        var user = _context.Users.FirstOrDefault(x => x.Id == userId);
        var symbol = await _context.Symbols.FirstOrDefaultAsync(x => x.Id == userInOutHistories.First().SymbolId);

        if (!changePeriods.Any() || user == null || symbol == null)
        {
            return null;
        }
        
        var workingDaysNumber = user.NumberWorkdays.GetValueOrDefault();
        var totalWorkingDaysInMonth = workingDaysNumber > 0 ? workingDaysNumber : daysNumberInMonth;
        var workingHoursPerDay = symbol.TimeTotal;
        double totalRealSalary = 0;
        double totalRealWorkingDays = 0;
        double contractualSalary = 0;
        double totalRegularWorkingDays = 0;
        
        foreach (var period in changePeriods)
        {
            var (realSalary, realWorkingDays, regularWorkingDays) = ProcessCalculateSalaryPeriod(
                userInOutHistories: userInOutHistories,
                overtimes: overtimes,
                estimatedSalary: period.EstimatedSalary,
                totalWorkingDaysInMonth: totalWorkingDaysInMonth,
                workingHoursPerDay: workingHoursPerDay,
                fromDate: period.FromDate,
                toDate: period.ToDate
            );
            var contractualSalaryPeriod = ProcessCalculateContractualSalary(userId ,contractualSalaries);
            contractualSalary += contractualSalaryPeriod;
            totalRealSalary += realSalary;
            totalRealWorkingDays += realWorkingDays;
            totalRegularWorkingDays += regularWorkingDays;
        }

        var allowanceAmount = ProcessCalculateAllowanceUser(userId, userAllowances, totalRegularWorkingDays);
        double deduceMealCost = ProcessDeduceMealCost(mealDetails, userId);
        var commission = userCommissions.FirstOrDefault(x => x.UserCode == user.Username);
        var userSalary = new Salary
        {
            Userid = userId,
            Month = month,
            Year = year,
            NumberOfWorkingDays = totalRealWorkingDays,
            BaseSalary = totalRealSalary.RoundToThousand(),
            AllowanceAmount = allowanceAmount.RoundToThousand(),
            ContractualSalaryAmount = contractualSalary.RoundToThousand(),
            DeduceMealCost = deduceMealCost.RoundToThousand(),
            SaleCommission = commission?.Commission ?? 0d
        };
        userSalary.CalculateRemainingSalary();
        return userSalary;
    }

    private double ProcessDeduceMealCost(List<NumberOfMealDetail> mealDetails, int userId)
    {
        double mealCost = 20000;
        var deduceMealCount = (mealDetails.Where(x => x.UserId == userId).ToList()).Count();
        return deduceMealCount * mealCost;
    }
    
    private double CalculateOverTimeWorkingHours(
        IEnumerable<ProcedureRequestOvertime> overtimes, 
        DateTime fromDate, 
        DateTime toDate)
    {
        var periodOverTimes = overtimes.Where(x =>
                x.FromAt.Date >= fromDate.Date &&
                x.ToAt.Date <= toDate.Date
            )
            .Select(x => (x.ToAt - x.FromAt).TotalHours * x.Coefficient)
            .ToList();

        if (!periodOverTimes.Any())
        {
            return 0;
        }
        
        return periodOverTimes.Sum();
    }

    private (
        double realSalary, 
        double realWorkingDays, 
        double regularWorkingDays
        ) ProcessCalculateSalaryPeriod(
        IEnumerable<InOutHistoryPeriodModel> userInOutHistories,
        IEnumerable<ProcedureRequestOvertime> overtimes,
        double estimatedSalary,
        double totalWorkingDaysInMonth,
        double workingHoursPerDay,
        DateTime fromDate,
        DateTime toDate)
    {
        // Calculate overtime working hours
        var overtimesWorkingHours = CalculateOverTimeWorkingHours(overtimes, fromDate, toDate);
            
        // Get histories by small period
        var periodHistories = userInOutHistories.Where(x => x.TimeIn.IsDateBetween(fromDate, toDate)).ToList();
            
        // Calculate Regular working hours 
        var regularWorkingHours = periodHistories.Sum(x => (x.AcceptedTimeOut - x.AcceptedTimeIn).TotalHours);
            
        // Calculate salary per day
        var salaryPerDay = estimatedSalary / totalWorkingDaysInMonth;
            
        // Calculate real working days based on total working hours calculated above
        var regularWorkingDays = regularWorkingHours / workingHoursPerDay;
        var overtimeWorkingDays = overtimesWorkingHours / workingHoursPerDay;

        // Total working days = Regular + overtime
        var totalWorkingDays = Math.Round(regularWorkingDays + overtimeWorkingDays, 2);
            
        // Real salary = salary per day * Total real working days
        var realSalary = salaryPerDay * totalWorkingDays;

        return (realSalary, totalWorkingDays, regularWorkingDays);
    }

    private double ProcessCalculateContractualSalary(
        int userId,
        IEnumerable<ContractBasedSalaryModel> contractBasedSalaries)
    {
        var userContractBasedSalaries = contractBasedSalaries
            .Where(x => x.UserId == userId)
            .Select(x => x.AmountPerPercent)
            .ToList();

        if (!userContractBasedSalaries.Any())
        {
            return 0;
        }

        return userContractBasedSalaries.Sum();
    }

    private double ProcessCalculateAllowanceUser(
        int userId, 
        List<SalaryAllowanceModel> userAllowances,
        double totalRegularWorkingDays)
    {
        var qualifiedAllowances = userAllowances.Where(x =>
            x.UserId == userId &&
            totalRegularWorkingDays >= x.WorkingDaysFrom &&
            totalRegularWorkingDays <= x.WorkingDaysTo
        ).ToList();
        if (!qualifiedAllowances.Any())
        {
            return 0;
        }
        return qualifiedAllowances.Sum(x => Convert.ToDouble(x.Value));
    }

    public async Task<string> ExportToExcel(int month, int year)
    {
        var fileMapServer = $"Salary{DateTime.Now:yyyyMMddHHmmss}.xlsx";
        var folder = Path.Combine(Directory.GetCurrentDirectory(), @"ExportHistory\EXCEL");
        var pathSave = Path.Combine(folder, fileMapServer);

        var path = Path.Combine(Directory.GetCurrentDirectory(), @"Uploads\Excel\Salary.xlsx");

        var salaries = await GetSalariesByMonthQueryable(month, year).ToListAsync();

        await using var templateDocumentStream = File.OpenRead(path);
        using var package = new ExcelPackage(templateDocumentStream);
        var sheet = package.Workbook.Worksheets["Sheet1"];
        
        var index = 1;
        // Replace title
        var title = sheet.Cells[3, 1].Value.ToString()
            ?.Replace("[Month]", month.ToString("00"))
            .Replace("[Year]", year.ToString("00"));
        sheet.Cells[3, 1].Value = title;
        
        sheet.InsertRow(8, salaries.Count);
        var beginRowIndex = 8;
        var endRowIndex = beginRowIndex + salaries.Count - 1;
        foreach (var salary in salaries)
        {
            sheet.Cells[beginRowIndex, 1].Value = index;
            sheet.Cells[beginRowIndex, 2].Value = salary.Username;
            sheet.Cells[beginRowIndex, 3].Value = salary.FullName;
            sheet.Cells[beginRowIndex, 4].Value = salary.BankAccountNumber;
            sheet.Cells[beginRowIndex, 5].Value = salary.BankName;
            sheet.Cells[beginRowIndex, 6].Value = salary.NumberOfWorkingDays;
            sheet.Cells[beginRowIndex, 7].Value = salary.Salary;
            sheet.Cells[beginRowIndex, 8].Value = salary.ContractualSalary;
            sheet.Cells[beginRowIndex, 9].Value = salary.AllowanceAmount;
            sheet.Cells[beginRowIndex, 10].Value = salary.SaleCommission;
            sheet.Cells[beginRowIndex, 11].Value = salary.DeduceMealCost;
            sheet.Cells[beginRowIndex, 12].Value = salary.RemainingAmount;
            ExcelHelpers.Format_Border_Excel_Range(sheet.Cells[beginRowIndex, 1, beginRowIndex, 11]);
            sheet.Cells[beginRowIndex, 7, beginRowIndex, 12].Style.Numberformat.Format = "#,##0";
            index++;
            beginRowIndex++;
        }
        
        // Summary row
        var summaryRowIndex = endRowIndex + 1;
        sheet.Cells[summaryRowIndex, 6].Value = salaries.Sum(x => x.NumberOfWorkingDays);
        sheet.Cells[summaryRowIndex, 7].Value = salaries.Sum(x => x.Salary);
        sheet.Cells[summaryRowIndex, 8].Value = salaries.Sum(x => x.ContractualSalary);
        sheet.Cells[summaryRowIndex, 9].Value = salaries.Sum(x => x.AllowanceAmount);
        sheet.Cells[summaryRowIndex, 10].Value = salaries.Sum(x => x.SaleCommission);
        sheet.Cells[summaryRowIndex, 11].Value = salaries.Sum(x => x.DeduceMealCost);
        sheet.Cells[summaryRowIndex, 7, summaryRowIndex, 12].Style.Numberformat.Format = "#,##0";
        
        var totalRemainingAmount = salaries.Sum(x => x.RemainingAmount);
        sheet.Cells[summaryRowIndex, 12].Value = totalRemainingAmount;
        
        // Amount in words
        var amountInWords = totalRemainingAmount.ConvertFromDecimal();
        sheet.Cells[endRowIndex + 2, 4].Value = amountInWords;
        
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        await using var fs = new FileStream(pathSave, FileMode.Create);
        await package.SaveAsAsync(fs);

        return fileMapServer;
    }
    
    public async Task<string> ExportToPdf(int month, int year)
    {
        var htmlContent = await FileExtension.ReadContent(
            Directory.GetCurrentDirectory(),
            @"Uploads\Html",
            "Salary.html"
        );
        
        var htmlDoc = new HtmlDocument();

        htmlDoc.LoadHtml(htmlContent);
        // Get template section
        var rowTemplateElement = htmlDoc.GetElementbyId("table-row-template");

        // Remove template after get content
        rowTemplateElement.Remove();
        var mainSb = new StringBuilder(htmlDoc.DocumentNode.OuterHtml);

        var salaries = await GetSalariesByMonthQueryable(month, year).ToListAsync();
        var index = 1;
        double sumSalary = 0;
        double sumNumberOfWorkingDays = 0;
        double sumContractualSalary = 0;
        double sumAllowanceAmount = 0;
        double sumSaleCommission = 0;
        double sumDeduceMealCost = 0;
        double sumRemainingAmount = 0;
        
        var tblDetailSb = new StringBuilder();
        foreach (var salary in salaries)
        {
            var rowSb = new StringBuilder(rowTemplateElement.InnerHtml)
                .Replace("[Index]", index.ToString())
                .Replace("[UserName]", salary.Username) 
                .Replace("[FullName]", salary.FullName)
                .Replace("[BankAccountNumber]", salary.BankAccountNumber)
                .Replace("[BankName]", salary.BankName)
                .Replace("[NumberOfWorkingDays]", salary.NumberOfWorkingDays.ToString("n2"))
                .Replace("[Salary]", salary.Salary.ToString("n0"))
                .Replace("[ContractualSalary]", salary.ContractualSalary.ToString("n0"))
                .Replace("[AllowanceAmount]", salary.AllowanceAmount.ToString("n0"))
                .Replace("[SaleCommission]", salary.SaleCommission.ToString("n0"))
                .Replace("[DeduceMealCost]", salary.DeduceMealCost.ToString("n0"))
                .Replace("[RemainingAmount]", salary.RemainingAmount.ToString("n0"));
            tblDetailSb.Append(rowSb);
            index++;
            sumSalary += salary.Salary;
            sumNumberOfWorkingDays += salary.NumberOfWorkingDays;
            sumContractualSalary += salary.ContractualSalary;
            sumAllowanceAmount += salary.AllowanceAmount;
            sumSaleCommission += salary.SaleCommission;
            sumDeduceMealCost += salary.DeduceMealCost;
            sumRemainingAmount += salary.RemainingAmount;
        }
        
        // Mapping company information
        await _pdfGeneratorService.MappingCompany(mainSb);
        
        // Summary row
        mainSb.Replace("[TABLE_BODY_SECTION]", tblDetailSb.ToString())
            .Replace("[Month]", month.ToString("00"))
            .Replace("[Year]", year.ToString("00"))
            .Replace("[SumSalary]", sumSalary.ToString("n0"))
            .Replace("[SumNumberOfWorkingDays]", sumNumberOfWorkingDays.ToString("n2"))
            .Replace("[SumContractualSalary]", sumContractualSalary.ToString("n0"))
            .Replace("[SumAllowanceAmount]", sumAllowanceAmount.ToString("n0"))
            .Replace("[SumSaleCommission]", sumSaleCommission.ToString("n0"))
            .Replace("[SumDeduceMealCost]", sumDeduceMealCost.ToString("n0"))
            .Replace("[SumRemainingAmount]", sumRemainingAmount.ToString("n0"))
            .Replace("[AmountInWord]", sumRemainingAmount.ConvertFromDecimal());
        

        return ExcelHelpers.ConvertUseDinkLandscape(
            mainSb.ToString(),
            _converterPDF,
            Directory.GetCurrentDirectory(),
            "Salary"
        );
    }

    private IQueryable<SalaryReportModel> GetSalariesByMonthQueryable(int month, int year)
    {
        return (from salary in _context.Salaries
            join user in _context.Users on salary.Userid equals user.Id
            where salary.Month == month && salary.Year == year
            select new SalaryReportModel()
            {
                Username = user.Username,
                FullName = user.FullName,
                BankAccountNumber = user.BankAccount,
                BankName = user.Bank,
                NumberOfWorkingDays = salary.NumberOfWorkingDays,
                Salary = salary.BaseSalary,
                AllowanceAmount = salary.AllowanceAmount,
                ContractualSalary = salary.ContractualSalaryAmount,
                DeduceMealCost = salary.DeduceMealCost,
                RemainingAmount = salary.RemainingAmount,
                SaleCommission = salary.SaleCommission
            });
    }

    private async Task<List<UserCommissionModel>> CollectBillCommission(int month, int year)
    {
        var directorRoleCode = "GDKD";
        var saleRoleCode = "SALES";
        
        var userRoles = await _context.UserRoles.Where(x => x.Code == directorRoleCode || x.Code == saleRoleCode).ToListAsync();
        var directorRoleId = userRoles.FirstOrDefault(x => x.Code == directorRoleCode)?.Id;
        
        // Get Commission from db
        var saleCommission = 100000d;
        var directorCommisson = 80000d;
        
        var fromAt = new DateTime(year, month, 1);
        var toAt = new DateTime(year, month, DateTime.DaysInMonth(year, month));

        // Get bills in month
        var billQueryable = _context.Bills.Where(x => x.CreatedDate.Date >= fromAt.Date && x.CreatedDate.Date < toAt.Date);
        var detailBills = await _context.BillDetails.Join(
                billQueryable,
                detail => detail.BillId,
                bill => bill.Id,
                (detail, bill) => new
                {
                    bill.UserCode,
                    Quantity = Convert.ToDouble(detail.Quantity),
                })
            .ToListAsync();

        // Refund bill
        var refundBills = await _context.BillDetailRefunds.Join(
            billQueryable,
            detail => detail.BillId,
            bill => bill.Id,
            (detail, bill) => new
            {
                bill.UserCode,
                Quantity = -detail.Quantity
            }).ToListAsync();
        
        // Sum revenue by user
        var monthlyRevenues = detailBills.Concat(refundBills)
            .GroupBy(x => x.UserCode)
            .Select(x => new UserCommissionModel
            {
                UserCode = x.Key,
                Commission = x.Sum(p => p.Quantity) * saleCommission
            }).ToList();

        // Recalculate commission for director
        var directorUserCodes = await _context.Users
            .Where(x =>
                directorRoleId.HasValue && 
                ("," + x.UserRoleIds + ",").Contains("," + directorRoleId)
            )
            .Select(x => x.Username)
            .ToListAsync();
        
        var saleCommissionAmount = monthlyRevenues.Where(x => !directorUserCodes.Contains(x.UserCode)).Sum(x => x.Commission);
        var directorUsers = monthlyRevenues.Where(x => directorUserCodes.Contains(x.UserCode));
        foreach (var directorUser in directorUsers)
        {
            directorUser.Commission += saleCommissionAmount * directorCommisson;
        }
        return monthlyRevenues;
    }
}