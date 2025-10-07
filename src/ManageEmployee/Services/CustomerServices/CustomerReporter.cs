using Common.Constants;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.CustomerModels;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.SearchModels;
using ManageEmployee.Entities.CustomerEntities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Customers;
using ManageEmployee.Services.Interfaces.Excels;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace ManageEmployee.Services.CustomerServices;

public class CustomerReporter : ICustomerReporter
{
    private readonly ApplicationDbContext _context;
    private readonly IExcelService _excelService;

    public CustomerReporter(ApplicationDbContext context,
        IExcelService excelService)
    {
        _context = context;
        _excelService = excelService;
    }

    public async Task<string> ExportExcel(CustomersSearchViewModel param, int userId)
    {
        string fileMapServer = $"DanhSachKhachHang_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx",
                folder = Path.Combine(Directory.GetCurrentDirectory(), @"ExportHistory\\EXCEL"),
                pathSave = Path.Combine(folder, fileMapServer);
        param.ExportExcel = 1;

        var data = await GetAllDataExport(param, userId);
        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads\\Excel\\DanhSachKhachHang.xlsx");
        using (FileStream templateDocumentStream = System.IO.File.OpenRead(fullPath))
        {
            using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets["Sheet1"];
                int nRowBegin = 7;
                int rowIdx = nRowBegin;
                var rows = data.Data;

                int provinceColIndex = 10;
                int districtColIndex = 11;
                int wardColIndex = 12;

                await _excelService.PrepareLocationRawSheetDataExcel(package, sheet, nRowBegin, rows.Count, provinceColIndex, districtColIndex, wardColIndex);
                var custonerIds = data.Data.Select(x => x.Id);
                var customerTaxs = await _context.CustomerTaxInformations.Where(x => custonerIds.Contains(x.CustomerId)).ToListAsync();

                if (rows.Count > 0)
                {
                    int i = 0;
                    foreach (var row in rows)
                    {
                        i++;
                        sheet.Cells[rowIdx, 1].Value = i.ToString();
                        sheet.Cells[rowIdx, 2].Value = row.Avatar;
                        sheet.Cells[rowIdx, 3].Value = row.Code;
                        sheet.Cells[rowIdx, 4].Value = row.Name;
                        sheet.Cells[rowIdx, 5].Value = row.Phone;
                        sheet.Cells[rowIdx, 6].Value = row.Facebook;
                        sheet.Cells[rowIdx, 7].Value = row.Email;
                        sheet.Cells[rowIdx, 8].Value = StringHelpers.GenderVI(row.Gender);
                        sheet.Cells[rowIdx, 9].Value = (row.Birthday ?? DateTime.Now).ToString("dd/MM/yyyy");

                        // Location
                        sheet.Cells[rowIdx, provinceColIndex].Value = row.ProvinceName;
                        sheet.Cells[rowIdx, districtColIndex].Value = row.DistrictName;
                        sheet.Cells[rowIdx, wardColIndex].Value = row.WardName;

                        sheet.Cells[rowIdx, 13].Value = row.Address;
                        sheet.Cells[rowIdx, 14].Value = row.IdentityCardNo;

                        var customerTax = customerTaxs.Find(x => x.CustomerId == row.Id);
                        sheet.Cells[rowIdx, 15].Value = customerTax?.CompanyName;
                        sheet.Cells[rowIdx, 16].Value = customerTax?.TaxCode;
                        sheet.Cells[rowIdx, 17].Value = customerTax?.Address;
                        sheet.Cells[rowIdx, 18].Value = customerTax?.Phone;
                        sheet.Cells[rowIdx, 19].Value = customerTax?.AccountNumber;
                        sheet.Cells[rowIdx, 20].Value = customerTax?.Bank;

                        sheet.Cells[rowIdx, 21].Value = row.DebitCode;
                        sheet.Cells[rowIdx, 22].Value = row.DebitDetailCodeFirst;
                        sheet.Cells[rowIdx, 23].Value = row.DebitDetailCodeSecond;

                        rowIdx++;
                    }
                }

                var sexual = sheet.Cells[nRowBegin, 9, rowIdx, 9].DataValidation.AddListDataValidation();
                sexual.Formula.Values.Add(StringHelpers.GenderVI(GenderEnum.Male));
                sexual.Formula.Values.Add(StringHelpers.GenderVI(GenderEnum.Female));

                rowIdx--;
                int nCol = 23;
                if (rowIdx >= nRowBegin)
                {
                    sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                    sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                }

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                using (FileStream fs = new FileStream(pathSave, FileMode.Create))
                {
                    await package.SaveAsAsync(fs);
                }
            }
        }

        return fileMapServer;
    }

    private async Task<PagingResult<CustomerModel>> GetAllDataExport(CustomersSearchViewModel param, int userId)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            string userRoleIds = "," + user?.UserRoleIds + ",";
            var roleCodes = await _context.UserRoles.Where(x => userRoleIds.Contains("," + x.Id.ToString() + ",")).Select(x => x.Code).ToListAsync();
            var listCustomers = (from p in _context.Customers
                                 join province in _context.Provinces on p.ProvinceId equals province.Id into provinceDefault
                                 from province in provinceDefault.DefaultIfEmpty()
                                 join district in _context.Districts on p.DistrictId equals district.Id into districtDefault
                                 from district in districtDefault.DefaultIfEmpty()
                                 join ward in _context.Wards on p.WardId equals ward.Id into wardDefault
                                 from ward in wardDefault.DefaultIfEmpty()
                                 where
                                     (string.IsNullOrEmpty(param.SearchText) ||
                                     p.Code.ToLower().Trim().Contains(param.SearchText.ToLower()) ||
                                     p.Phone.ToLower().Trim().Contains(param.SearchText.ToLower()) ||
                                     p.Name.ToLower().Trim().Contains(param.SearchText.ToLower()) ||
                                     p.IdentityCardNo.Trim().Contains(param.SearchText)
                                     )
                                     && (param.Gender == null || p.Gender == param.Gender)

                                     && (param.FromDate == null || p.CreateAt >= param.FromDate)
                                     && (param.ToDate == null || p.CreateAt <= param.ToDate)
                                     && p.Type == param.Type
                                     && (param.EmployeeId == null || param.EmployeeId == 0 || p.UserCreated == param.EmployeeId)

                                 select new CustomerModel
                                 {
                                     Id = p.Id,
                                     Code = p.Code,
                                     Avatar = p.Avatar,
                                     Name = p.Name,
                                     Address = p.Address,
                                     IdentityCardAddressInCard = p.IdentityCardAddressInCard,
                                     Birthday = p.Birthday,
                                     DistrictId = p.DistrictId,
                                     Email = p.Email,
                                     Facebook = p.Facebook,
                                     Gender = p.Gender,
                                     IdentityCardDistrictId = p.IdentityCardDistrictId,
                                     IdentityCardNo = p.IdentityCardNo,
                                     IdentityCardProvinceId = p.IdentityCardProvinceId,
                                     IdentityCardWardId = p.IdentityCardWardId,
                                     WardId = p.WardId,
                                     Phone = p.Phone,
                                     ProvinceId = p.ProvinceId,
                                     IdentityCardPlaceOfPermanent = p.IdentityCardPlaceOfPermanent,
                                     IdentityCardIssueDate = p.IdentityCardIssueDate,
                                     IdentityCardIssuePlace = p.IdentityCardIssuePlace,
                                     IdentityCardValidUntil = p.IdentityCardValidUntil,
                                     DebitCode = p.DebitCode,
                                     DebitDetailCodeFirst = p.DebitDetailCodeFirst,
                                     DebitDetailCodeSecond = p.DebitDetailCodeSecond,
                                     CustomerClassficationId = p.CustomerClassficationId,
                                     UserCreated = p.UserCreated,
                                     ProvinceName = province != null ? province.Name : string.Empty,
                                     DistrictName = district != null ? district.Name : string.Empty,
                                     WardName = ward != null ? ward.Name : string.Empty,
                                 });
            if(param.StatusId != null)
            {
                var customerIds = await _context.CustomerContactHistories.Where(x => x.StatusId == param.StatusId).Select(X => X.CustomerId).Distinct().ToListAsync();
                listCustomers = listCustomers.Where(x => customerIds.Contains(x.Id));

            }
            if (param.JobId != null)
            {
                var customerIds = await _context.CustomerContactHistories.Where(x => x.JobsId == param.JobId).Select(X => X.CustomerId).Distinct().ToListAsync();
                listCustomers = listCustomers.Where(x => customerIds.Contains(x.Id));
            }
            if (param.EmployeeId == -1)
            {
                listCustomers = listCustomers.Where(x => x.UserCreated == null);
            }
            else
            {
                if (roleCodes.Contains(UserRoleConst.SuperAdmin) || roleCodes.Contains(UserRoleConst.AdminBranch) || roleCodes.Contains(UserRoleConst.KeToanTruong))
                {
                }
                else if (roleCodes.Contains(UserRoleConst.TruongPhong))
                {
                    var userIds = await _context.Users.Where(x => x.DepartmentId == user.DepartmentId).Select(x => x.Id).ToListAsync();
                    listCustomers = listCustomers.Where(x => userIds.Contains(x.UserCreated ?? 0));
                }
                else
                {
                    listCustomers = listCustomers.Where(x => x.UserCreated == userId);
                }
            }

            return new PagingResult<CustomerModel>()
            {
                Data = await listCustomers.ToListAsync()
            };
        }
        catch
        {
            return new PagingResult<CustomerModel>()
            {
                CurrentPage = param.Page,
                PageSize = param.PageSize,
                TotalItems = 0,
                Data = new List<CustomerModel>()
            };
        }
    }

    public async Task<string> ImportExcel(List<CustomerImport> datas, int UserId, int type, string roles)
    {
        string result = "";
        var customerAdds = new List<Customer>();
        var customerUpdates = new List<Customer>();
        foreach (var data in datas)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Code == data.Code && x.Type == type);
            if (customer == null)
            {
                customer = new Customer();
            }
            customer.Avatar = data.Avatar;
            customer.Code = data.Code;
            customer.Name = data.Name;
            customer.Phone = data.Phone;
            customer.Facebook = data.Facebook;
            customer.Email = data.Email;
            customer.Gender = data.Gender;
            customer.Birthday = data.Birthday;

            // Location
            customer.Address = data.Address;
            var province = await _context.Provinces.FirstOrDefaultAsync(x => x.Name == data.ProvinceName);
            customer.ProvinceId = province?.Id;

            if (province != null)
            {
                var district = await _context.Districts.FirstOrDefaultAsync(x => x.Name == data.DistrictName && x.ProvinceId == province.Id);
                customer.DistrictId = district?.Id;
                if (district != null)
                {
                    var ward = await _context.Wards.FirstOrDefaultAsync(x => x.Name == data.WardName && x.DistrictId == district.Id);
                    customer.WardId = ward?.Id;
                }
            }
            customer.IdentityCardNo = data.IdentityCardNo;
            customer.DebitCode = data.DebitCode;
            customer.DebitDetailCodeFirst = data.DebitDetailCodeFirst;
            customer.DebitDetailCodeSecond = data.DebitDetailCodeSecond;
            customer.UserUpdated = UserId;

            if (customer.Id == 0)
            {
                customer.UserCreated = UserId;
                customer.Type = type;
                customerAdds.Add(customer);
            }
            else
            {
                if (customer.UserCreated != UserId && !roles.Contains(UserRoleConst.SuperAdmin))
                {
                    result += "Mã khách hàng " + data.Code + " đã tồn tại; ";
                    continue;
                }
                var customerCheck = customerUpdates.Find(x => x.Id == customer.Id);
                if (customerCheck != null)
                    continue;

                customerUpdates.Add(customer);
            }
        }
        if (!string.IsNullOrEmpty(result))
        {
            return result;
        }
        await _context.Customers.AddRangeAsync(customerAdds);
        await _context.SaveChangesAsync();

        var customerTaxAdds = new List<CustomerTaxInformation>();
        var customerTaxUpdates = new List<CustomerTaxInformation>();

        foreach (var customer in customerAdds)
        {
            var data = datas.Find(x => x.Code == customer.Code && x.Type == type);
            if (data is null)
                continue;

            var dataTax = new CustomerTaxInformation
            {
                CompanyName = data.TaxCompanyName,
                TaxCode = data.TaxTaxCode,
                Address = data.TaxAddress,
                Phone = data.TaxPhone,
                Bank = data.TaxBank,
                AccountNumber = data.TaxAccountNumber,
                CustomerId = customer.Id,
            };

            customerTaxAdds.Add(dataTax);
        }
        foreach (var customer in customerUpdates)
        {
            var data = datas.Find(x => x.Code == customer.Code && x.Type == type);
            if (data is null)
                continue;

            var dataTax = new CustomerTaxInformation
            {
                CompanyName = data.TaxCompanyName,
                TaxCode = data.TaxTaxCode,
                Address = data.TaxAddress,
                Phone = data.TaxPhone,
                Bank = data.TaxBank,
                AccountNumber = data.TaxAccountNumber,
                CustomerId = customer.Id,
            };

            var dataTaxCheck = await _context.CustomerTaxInformations.FirstOrDefaultAsync(x => x.CustomerId == customer.Id);
            if (dataTaxCheck is null)
            {
                dataTax.Id = 0;
                dataTax.CustomerId = customer.Id;
                customerTaxAdds.Add(dataTax);
            }
            else
            {
                dataTax.Id = dataTaxCheck.Id;
                customerTaxUpdates.Add(dataTax);
            }
        }
        await _context.CustomerTaxInformations.AddRangeAsync(customerTaxAdds);
        _context.Customers.UpdateRange(customerUpdates);
        _context.CustomerTaxInformations.UpdateRange(customerTaxUpdates);
        await _context.SaveChangesAsync();

        return result;
    }

    public async Task<object> ChartBirthdayForCustomer(int userId, int type)
    {
        CustomersSearchViewModel param = new CustomersSearchViewModel();
        param.Type = type;
        var users = await GetAllDataExport(param, userId);

        var birthDayOfUsers = users.Data
            .GroupBy(x => x.Birthday.HasValue ? x.Birthday.Value.Month : -1)
            .Select(x => new
            {
                Month = x.Key,
                Male = x.Count(s => s.Gender == GenderEnum.Male),
                Female = x.Count(s => s.Gender == GenderEnum.Female)
            })
            .ToDictionary(x => x.Month);
        List<object> distributeByMonth = new();

        for (int i = 1; i <= 12; i++)
        {
            distributeByMonth.Add(new
            {
                Month = i,
                Male = birthDayOfUsers.ContainsKey(i) ? birthDayOfUsers[i].Male : 0,
                Female = birthDayOfUsers.ContainsKey(i) ? birthDayOfUsers[i].Female : 0,
            });
        }

        var result = new
        {
            TotalItems = birthDayOfUsers.Count,
            BirthDayOfUsers = distributeByMonth
        };
        return result;
    }
}