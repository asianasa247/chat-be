using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.UserModels;
using ManageEmployee.Entities.AllowanceEntities;
using ManageEmployee.Entities.InOutEntities;
using ManageEmployee.Entities.LedgerEntities;
using ManageEmployee.Entities.ProcedureEntities;
using ManageEmployee.Entities.SalaryEntities;
using ManageEmployee.Services.Interfaces.Ledgers;
using ManageEmployee.Services.Interfaces.Users;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace ManageEmployee.Services.UserServices;
public class UserSalaryService : IUserSalaryService
{
    private readonly ApplicationDbContext _context;
    private readonly ILedgerService _ledgerServices;

    private List<User_SalaryModel> _listUser;
    private List<InOutHistory> _listInOut;
    private List<SalarySocial> _listSalarySocial;
    private List<Allowance> _allowances;
    private List<AllowanceUser> _allowanceUsers;
    private List<P_SalaryAdvance_Item> _salaryItems;
    public UserSalaryService(ApplicationDbContext context, ILedgerService ledgerServices)
    {
        _context = context;
        _ledgerServices = ledgerServices;
    }
    public async Task SetData(int Month)
    {
        _listInOut = await _context.InOutHistories.Where(x => x.TimeIn.Value.Month == Month && x.TimeIn.Value.Year == DateTime.Today.Year).ToListAsync();
        _listUser = await (from coa in _context.Users
                           join posi in _context.PositionDetails on coa.PositionDetailId equals posi.Id
                           where !coa.IsDelete
                           select new User_SalaryModel()
                           {
                               Id = coa.Id,
                               PositionName = posi.Name,
                               FullName = coa.FullName ?? "",
                               Salary = coa.Salary ?? 0,
                               ContractTypeId = coa.ContractTypeId,
                               DepartmentId = coa.DepartmentId ?? 0,
                               NumberWorkdays = coa.NumberWorkdays,
                               SocialInsuranceSalary = coa.SocialInsuranceSalary ?? 0,
                               IsManager = posi.IsManager,
                               IsProbation = coa.ProbationFromAt >= DateTime.Today && coa.ProbationFromAt <= DateTime.Today,
                               SalaryPercentage = coa.SalaryPercentage
                           }).ToListAsync();
        _listSalarySocial = await _context.SalarySocials.Where(x => x.Code != "LUONGNHANVIEN" && x.Code != "LUONGQUANLY").OrderBy(x => x.Order).ToListAsync();
        _allowances = await _context.Allowances.Where(x => !x.IsDelete).ToListAsync();
        _allowanceUsers = await _context.AllowanceUsers.Where(x => !x.IsDelete).ToListAsync();
        var pSalaryIds = await _context.P_SalaryAdvance.Where(x => x.Date != null && x.Date.Value.Month == Month && x.Date.Value.Year == DateTime.Now.Year && x.IsFinished).Select(x => x.Id).ToListAsync();
        _salaryItems = await _context.P_SalaryAdvance_Item.Where(x => pSalaryIds.Contains(x.P_SalaryAdvanceId)).ToListAsync();

    }
    public async Task<List<User_SalaryModel>> GetListUserSalary(int month, int isInternal)
    {
        await SetData(month);
        List<User_SalaryModel> listOut = await GetListUserSalaryForDeparterment(isInternal);
        return listOut;

    }
    private async Task<List<User_SalaryModel>> GetListUserSalaryForDeparterment(int isInternal)
    {

        var listDepartment = await _context.Departments.Where(x => !x.isDelete).ToListAsync();
        char sttDep = 'A';
        List<User_SalaryModel> listOut = new List<User_SalaryModel>();
        foreach (var department in listDepartment)
        {
            User_SalaryModel itemTong = new User_SalaryModel();
            itemTong.salarySocial = new List<SalarySocialModel>();
            itemTong.FullName = department.Name;
            itemTong.listChild = new List<User_SalaryModel>();
            var listUserForDep = _listUser.Where(X => X.DepartmentId == department.Id).ToList();
            if (listUserForDep.Any())
            {
                itemTong.listChild.AddRange(CalculationSalary(listUserForDep, isInternal));
            }

            foreach (var salarySocial in _listSalarySocial)
            {
                SalarySocialModel socialModel = new SalarySocialModel();
                socialModel.Code = salarySocial.Code;
                socialModel.ValueCompany = listUserForDep.Sum(x => x.salarySocial.Where(x => x.Code == salarySocial.Code).Sum(x => x.ValueCompany));
                itemTong.salarySocial.Add(socialModel);
            }
            {
                SalarySocialModel socialModel = new SalarySocialModel();
                socialModel.Code = "Tong";
                socialModel.ValueCompany = itemTong.salarySocial.Sum(x => x.ValueCompany);
                itemTong.salarySocial.Add(socialModel);
            }

            foreach (var salarySocial in _listSalarySocial)
            {
                if (salarySocial.ValueUser == 0)
                    continue;
                SalarySocialModel socialModel = new SalarySocialModel();
                socialModel.Code = salarySocial.Code;
                socialModel.ValueUser = listUserForDep.Sum(x => x.salarySocial.Where(x => x.Code == salarySocial.Code).Sum(x => x.ValueUser));
                itemTong.salarySocial.Add(socialModel);

            }
            {
                SalarySocialModel socialModel = new SalarySocialModel();
                socialModel.Code = "Tong";
                socialModel.ValueUser = itemTong.salarySocial.Sum(x => x.ValueUser);
                itemTong.salarySocial.Add(socialModel);
            }

            itemTong.allowances = new List<AllowanceUserModel>();
            foreach (var allowance in _allowances)
            {
                AllowanceUserModel allowanceUserModel = new AllowanceUserModel();
                allowanceUserModel.Code = allowance.Code;
                allowanceUserModel.Value = itemTong.listChild.Sum(x => x.allowances.Where(x => x.Code == allowance.Code).Sum(x => x.Value));
                itemTong.allowances.Add(allowanceUserModel);
            }

            itemTong.SoThuTu = sttDep.ToString();
            itemTong.SocialInsuranceSalary = listUserForDep.Sum(x => x.SocialInsuranceSalary);
            itemTong.Salary = listUserForDep.Sum(x => x.Salary);
            itemTong.SalaryTotal = listUserForDep.Sum(x => x.SalaryTotal);
            itemTong.DayInOut = listUserForDep.Sum(x => x.DayInOut);
            itemTong.SalaryReal = listUserForDep.Sum(x => x.SalaryReal);
            itemTong.ThueTNCN = listUserForDep.Sum(x => x.ThueTNCN);
            itemTong.TamUng = listUserForDep.Sum(x => x.TamUng);
            itemTong.SalarySend = listUserForDep.Sum(x => x.SalarySend);
            listOut.Add(itemTong);
            sttDep++;
        }
        return listOut.ToList();

    }
    private List<User_SalaryModel> CalculationSalary(List<User_SalaryModel> listUserForDep, int isInternal, bool isAccountant = false)
    {
        int i = 0;
        List<User_SalaryModel> listOut = new List<User_SalaryModel>();
        foreach (var user in listUserForDep)
        {
            i++;
            user.DayInOut = _listInOut.Count(x => x.UserId == user.Id);
            user.SoThuTu = i.ToString();
            user.salarySocial = new List<SalarySocialModel>();

            foreach (var salarySocial in _listSalarySocial)
            {
                SalarySocialModel socialModel = new SalarySocialModel();
                socialModel.Code = salarySocial.Code;
                socialModel.ValueCompany = user.SocialInsuranceSalary * salarySocial.ValueCompany / 100;
                user.salarySocial.Add(socialModel);
            }
            {
                SalarySocialModel socialModel = new SalarySocialModel();
                socialModel.Code = "Tong";
                socialModel.ValueCompany = user.salarySocial.Sum(x => x.ValueCompany);
                user.salarySocial.Add(socialModel);
            }
            foreach (var salarySocial in _listSalarySocial)
            {
                if (salarySocial.ValueUser == 0)
                    continue;
                SalarySocialModel socialModel = new SalarySocialModel();
                socialModel.Code = salarySocial.Code;
                socialModel.ValueUser = user.SocialInsuranceSalary * salarySocial.ValueUser / 100;
                user.salarySocial.Add(socialModel);
            }
            {
                SalarySocialModel socialModel = new SalarySocialModel();
                socialModel.Code = "Tong";
                socialModel.ValueUser = user.salarySocial.Sum(x => x.ValueUser);
                user.salarySocial.Add(socialModel);
            }

            user.allowances = new List<AllowanceUserModel>();
            foreach (var allowance in _allowances)
            {
                AllowanceUserModel allowanceUserModel = new AllowanceUserModel();
                allowanceUserModel.Code = allowance.Code;
                allowanceUserModel.Value = decimal.ToDouble(_allowanceUsers.FirstOrDefault(x => x.AllowanceId == allowance.Id && x.UserId == user.Id)?.Value ?? 0);
                user.allowances.Add(allowanceUserModel);
            }

            if (user.IsProbation)
            {
                user.Salary = user.Salary * (user.SalaryPercentage ?? 0);
            }

            if (user.NumberWorkdays > 0)
            {
                if (isInternal == 3)
                    user.SalaryReal = user.Salary / (user.NumberWorkdays ?? 1) * (user.DayInOut ?? 0);
                else
                    user.SalaryReal = user.SocialInsuranceSalary / (user.NumberWorkdays ?? 1) * (user.DayInOut ?? 0);
            }
            user.TamUng = _salaryItems.Where(x => x.UserId == user.Id).Sum(x => (double)x.Value);

            user.SalarySend = user.SalaryReal - (user.salarySocial.LastOrDefault()?.ValueUser ?? 0) - user.ThueTNCN - user.TamUng + user.allowances.Sum(x => x.Value);
            if (isAccountant)
            {
                user.SalarySend = user.SalarySend - user.TamUng;
            }
            listOut.Add(user);
        }
        return listOut;
    }
    public async Task<string> ExportSalary(int month, int isInternal)
    {
        List<User_SalaryModel> datas = await GetListUserSalary(month, isInternal);
        if (datas.Any())
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Excel/BangLuong.xlsx");

            string _fileMapServer = $"BangLuong_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx",
                folder = Path.Combine(Directory.GetCurrentDirectory(), @"ExportHistory\\EXCEL"),
                _pathSave = Path.Combine(folder, _fileMapServer);
            var allowances = await _context.Allowances.Where(x => !x.IsDelete).ToListAsync();

            using (FileStream templateDocumentStream = File.OpenRead(path))
            {
                using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
                {
                    ExcelWorksheet sheet = package.Workbook.Worksheets[0];
                    int rowIdx = 9, nRowBegin = 9;
                    int nCol = 5;
                    sheet.Cells["K4"].Value = "Tháng " + month + " năm " + DateTime.Today.Year;
                    foreach (var allowance in allowances)
                    {
                        sheet.Cells[rowIdx - 2, nCol].Value = allowance.Name;
                        nCol++;
                    }
                    foreach (User_SalaryModel lo in datas)
                    {
                        sheet.Cells[rowIdx, 1].Value = lo.SoThuTu;
                        sheet.Cells[rowIdx, 2].Value = lo.FullName;
                        sheet.Cells[rowIdx, 3].Value = lo.PositionName;
                        if (isInternal == 2)
                            sheet.Cells[rowIdx, 4].Value = lo.SocialInsuranceSalary;
                        else
                            sheet.Cells[rowIdx, 4].Value = lo.Salary;

                        nCol = 5;
                        foreach (var allowance in lo.allowances)
                        {
                            sheet.Cells[rowIdx, nCol].Value = allowance.Value;
                            nCol++;
                        }
                        sheet.Cells[rowIdx, nCol].Value = lo.SalaryTotal;
                        sheet.Cells[rowIdx, nCol + 1].Value = lo.DayInOut;
                        sheet.Cells[rowIdx, nCol + 2].Value = lo.SalaryReal;
                        sheet.Cells[rowIdx, nCol + 3].Value = lo.SocialInsuranceSalary;
                        nCol = nCol + 4;
                        foreach (var chiPhi in lo.salarySocial)
                        {
                            sheet.Cells[rowIdx, nCol].Value = chiPhi.ValueCompany > 0 ? chiPhi.ValueCompany : chiPhi.ValueUser;
                            nCol++;
                        }

                        nCol--;
                        sheet.Cells[rowIdx, nCol + 1].Value = lo.ThueTNCN;
                        sheet.Cells[rowIdx, nCol + 2].Value = lo.TamUng;
                        sheet.Cells[rowIdx, nCol + 3].Value = lo.SalarySend;
                        sheet.Cells[rowIdx, nCol + 4].Value = "";
                        rowIdx++;
                        foreach (User_SalaryModel loChild in lo.listChild)
                        {
                            sheet.Cells[rowIdx, 1].Value = loChild.SoThuTu;
                            sheet.Cells[rowIdx, 2].Value = loChild.FullName;
                            sheet.Cells[rowIdx, 3].Value = loChild.PositionName;
                            sheet.Cells[rowIdx, 4].Value = loChild.SocialInsuranceSalary;
                            nCol = 5;
                            foreach (var allowance in loChild.allowances)
                            {
                                sheet.Cells[rowIdx, nCol].Value = allowance.Value;
                                nCol++;
                            }
                            sheet.Cells[rowIdx, nCol].Value = loChild.SalaryTotal;
                            sheet.Cells[rowIdx, nCol + 1].Value = loChild.DayInOut;
                            sheet.Cells[rowIdx, nCol + 2].Value = loChild.SalaryReal;
                            sheet.Cells[rowIdx, nCol + 3].Value = loChild.SocialInsuranceSalary;
                            nCol = nCol + 4;
                            foreach (var chiPhi in loChild.salarySocial)
                            {
                                sheet.Cells[rowIdx, nCol].Value = chiPhi.ValueCompany > 0 ? chiPhi.ValueCompany : chiPhi.ValueUser;
                                nCol++;
                            }

                            nCol--;
                            sheet.Cells[rowIdx, nCol + 1].Value = loChild.ThueTNCN;
                            sheet.Cells[rowIdx, nCol + 2].Value = loChild.TamUng;
                            sheet.Cells[rowIdx, nCol + 3].Value = loChild.SalarySend;
                            sheet.Cells[rowIdx, nCol + 4].Value = "";
                            rowIdx++;
                        }
                    }
                    rowIdx--;
                    if (rowIdx >= nRowBegin)
                    {
                        sheet.Cells[nRowBegin, 4, rowIdx, nCol + 3].Style.Numberformat.Format = "_(* #,##0_);_(* (#,##0);_(* \"-\"??_);_(@_)";

                        sheet.Cells[nRowBegin, 1, rowIdx, nCol + 4].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                        sheet.Cells[nRowBegin, 1, rowIdx, nCol + 4].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                        sheet.Cells[nRowBegin, 1, rowIdx, nCol + 4].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                        sheet.Cells[nRowBegin, 1, rowIdx, nCol + 4].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                        sheet.Cells[nRowBegin, 1, rowIdx, nCol + 4].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        sheet.Cells[nRowBegin, 1, rowIdx, nCol + 4].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        sheet.Cells[nRowBegin, 1, rowIdx, nCol + 4].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        sheet.Cells[nRowBegin, 1, rowIdx, nCol + 4].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    }
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    using (FileStream fs = new FileStream(_pathSave, FileMode.Create))
                    {
                        package.SaveAs(fs);
                    }
                }
            }

            return _fileMapServer;
        }
        else
        {
            throw new Exception("Không có dữ liệu xuất file");
        }
    }

    public async Task UpdateSalaryToAccountant(int month, int isInternal, int year)
    {
        await SetData(month);
        List<User_SalaryModel> listData = CalculationSalary(_listUser, isInternal, true);

        int maxOriginalVoucher = 0;

        var listSalarySocial = await _context.SalarySocials.Where(x => x.Code == "LUONGQUANLY" || x.Code == "LUONGNHANVIEN").ToListAsync();

        var ledgerExist = await _context.GetLedger(year, isInternal).AsNoTracking().Where(x => !x.IsDelete && x.Type == "KC"
                                                            && x.OrginalBookDate.Value.Year == year && x.OrginalBookDate.Value.Month == month).ToListAsync();

        if (ledgerExist != null && ledgerExist.Count > 0)
        {
            maxOriginalVoucher = ledgerExist.Max(x => int.Parse(x.OrginalVoucherNumber.Split('-').Last()));
        }
        maxOriginalVoucher++;

        var orderString = maxOriginalVoucher < 10 ? $"00{maxOriginalVoucher}" :
                    maxOriginalVoucher < 100 ? $"0{maxOriginalVoucher}" : maxOriginalVoucher.ToString();

        string orginalVoucherNumber = $"{"KC"}{(month < 10 ? "0" + month : month.ToString())}-{year.ToString().Substring(2, 2)}-{orderString}";

        foreach (var salarySocial in listSalarySocial)
        {
            Ledger ledger = new()
            {
                Month = month,
                BookDate = DateTime.Today,
                Type = "KC"
            };
            ledger.VoucherNumber = (month < 10 ? "0" + month : month.ToString()) + "/" + ledger.Type;
            ledger.IsVoucher = false;

            ledger.OrginalVoucherNumber = orginalVoucherNumber;
            ledger.Order = maxOriginalVoucher;

            ledger.OrginalBookDate = DateTime.Today;
            ledger.ReferenceBookDate = DateTime.Today;
            ledger.InvoiceDate = DateTime.Today;

            ledger.DebitCode = salarySocial.AccountDebit;
            ledger.DebitDetailCodeFirst = salarySocial.DetailDebit1;
            ledger.DebitDetailCodeSecond = salarySocial.DetailDebit2;
            ledger.CreditCode = salarySocial.AccountCredit;
            ledger.CreditDetailCodeFirst = salarySocial.DetailCredit1;
            ledger.CreditDetailCodeSecond = salarySocial.DetailCredit2;

            ledger.Quantity = 0;
            ledger.UnitPrice = 0;
            if (salarySocial.Code == "LUONGQUANLY")
                ledger.Amount = listData.Where(x => x.IsManager).Sum(x => x.SalarySend);
            else
                ledger.Amount = listData.Where(x => !x.IsManager).Sum(x => x.SalarySend);

            ledger.CreateAt = DateTime.Now;
            ledger.IsInternal = isInternal;
            ledger.OrginalCode = "";
            ledger.OrginalFullName = "";

            ledger.ReferenceVoucherNumber = "";
            ledger.ReferenceFullName = "";
            ledger.ReferenceAddress = "";
            ledger.InvoiceProductItem = "";
            ledger.ProjectCode = "";

            await _ledgerServices.Create(ledger, year);

        }
    }

}
