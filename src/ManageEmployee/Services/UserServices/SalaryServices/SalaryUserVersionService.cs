using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.UserModels.SalaryModels;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.SalaryEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Assets;
using ManageEmployee.Services.Interfaces.Users.Salaries;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace ManageEmployee.Services.UserServices.SalaryServices;

public class SalaryUserVersionService : ISalaryUserVersionService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;

    public SalaryUserVersionService(ApplicationDbContext context, IMapper mapper, IFileService fileService)
    {
        _context = context;
        _mapper = mapper;
        _fileService = fileService;
    }

    public async Task<PagingResult<SalaryUserVersionModel>> GetPaging(PagingRequestModel param)
    {
        if (param.Page < 1)
            param.Page = 1;

        var query = _context.SalaryUserVersions;
        var data = await query.Skip(param.PageSize * (param.Page - 1)).Take(param.PageSize).ToListAsync();
        var listOut = new List<SalaryUserVersionModel>();
        var contractTypes = await _context.ContractTypes.Where(x => x.TypeContract == TypeContractEnum.User).ToListAsync();
        foreach (var item in data)
        {
            var contractType = contractTypes.FirstOrDefault(x => x.Id == item.ContractTypeId);
            listOut.Add(new SalaryUserVersionModel
            {
                Id = item.Id,
                Code = item.Code,
                Note = item.Note,
                Date = item.Date,
                EffectiveTo = item.EffectiveTo,
                EffectiveFrom = item.EffectiveFrom,
                Percent = item.Percent,
                SalaryTo = item.SalaryTo,
                UserFullName = item.UserFullName,
                ContractTypeName = contractType?.Name,
                SocialInsuranceSalary = item.SocialInsuranceSalary
            });
        }
        var totalItem = await query.CountAsync();
        return new PagingResult<SalaryUserVersionModel>
        {
            Data = listOut,
            TotalItems = totalItem,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        };
    }

    public async Task<SalaryUserVersionDetailModel> GetDetail(int id)
    {
        return await _context.SalaryUserVersions.Where(x => x.Id == id).Select(x => _mapper.Map<SalaryUserVersionDetailModel>(x)).FirstOrDefaultAsync();
    }

    public async Task SetData(SalaryUserVersionUpdateModel param, int userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == param.UserId);
        if (user is null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }
        var data = _mapper.Map<SalaryUserVersion>(param);
        data.UpdatedAt = DateTime.Now;
        data.UserUpdated = userId;
        if (data.Id == 0)
        {
            data.CreatedAt = DateTime.Now;
            data.UserCreated = userId;
        }
        if (data.SalaryFrom == null)
        {
            data.SalaryFrom = user.Salary;
        }

        data.ContractTypeId = param.ContractTypeId;
        data.UserFullName = user.FullName;
        data.UserName = user.Username;

        _context.SalaryUserVersions.Update(data);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var data = await _context.SalaryUserVersions.FindAsync(id);
        if (data is null)
            throw new ErrorException(ErrorMessages.DataNotFound);
        _context.SalaryUserVersions.Remove(data);
        await _context.SaveChangesAsync();
    }

    public async Task ImportExcel(IFormFile file)
    {
        var path_exc = _fileService.Upload(file, "ExportHistory\\EXCEL");

        using (FileStream templateDocumentStream = File.OpenRead(path_exc))
        {
            using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets.First();
                var i = 5;
                var users = await _context.Users.ToListAsync();
                var listOut = new List<SalaryUserVersion>();

                while (sheet.Cells[i, 1].Value != null)
                {
                    var userName = sheet.Cells[i, 3].Value.ToString();
                    var user = users.FirstOrDefault(x => x.Username == userName);
                    SalaryUserVersion pro = new SalaryUserVersion()
                    {
                        Id = int.Parse(sheet.Cells[i, 1].Value.ToString()),
                        Code = sheet.Cells[i, 2].Value.ToString(),
                        UserName = user.Username,
                        UserFullName = user.FullName,
                        ContractTypeId = user.ContractTypeId,
                        SalaryFrom = user.Salary,
                        SalaryTo = double.Parse(sheet.Cells[i, 6].Value.ToString()),
                        Date = DateTime.Parse(sheet.Cells[i, 7].Value.ToString()),
                        Note = sheet.Cells[i, 8].Value.ToString(),
                        UserId = user.Id,
                    };
                    i++;
                    listOut.Add(pro);
                }

                var itemUpdates = listOut.Where(x => x.Id > 0).ToList();
                var itemUpdateIds = itemUpdates.Select(x => x.Id).ToList();
                var salaryUserVersionIds = await _context.SalaryUserVersions.Where(x => itemUpdateIds.Contains(x.Id)).Select(x => x.Id).ToListAsync();
                var salaryUserVersionIdToAdd = itemUpdateIds.Where(x => !salaryUserVersionIds.Contains(x));
                var salaryUserVersionToAdd = listOut.Where(x => salaryUserVersionIdToAdd.Contains(x.Id)).Select(x => new SalaryUserVersion
                {
                    Id = 0,
                    Code = x.Code,
                    UserName = x.UserName,
                    SalaryFrom = x.SalaryFrom,
                    SalaryTo = x.SalaryTo,
                    UserFullName = x.UserFullName,
                    ContractTypeId = x.ContractTypeId,
                    Date = x.Date,
                    Note = x.Note,
                    UserId = x.UserId,
                }).ToList();

                var itemAdds = listOut.Where(x => x.Id == 0).ToList();
                await _context.SalaryUserVersions.AddRangeAsync(itemAdds);
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task<string> ExportExcel()
    {
        try
        {
            var datas = await _context.SalaryUserVersions.ToListAsync();
            string fileName = "SalaryUserVersionTemplate.xlsx";
            int nCol = 8;

            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Excel/" + fileName);
            using (FileStream templateDocumentStream = File.OpenRead(path))
            {
                using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];
                    int nRowBegin = 5, nRow = 5;
                    var contracts = await _context.ContractTypes.ToListAsync();
                    foreach (var data in datas)
                    {
                        var contract = contracts.FirstOrDefault(x => x.Id == data.ContractTypeId);

                        worksheet.Cells[nRow, 1].Value = data.Id;
                        worksheet.Cells[nRow, 2].Value = data.Code;
                        worksheet.Cells[nRow, 3].Value = data.UserName;
                        worksheet.Cells[nRow, 4].Value = data.UserFullName;
                        worksheet.Cells[nRow, 5].Value = contract?.Name;
                        worksheet.Cells[nRow, 6].Value = data.SalaryTo;
                        worksheet.Cells[nRow, 7].Value = data.Date.ToString("dd/MM/yyyy");
                        worksheet.Cells[nRow, 8].Value = data.Note;
                        nRow++;
                    }
                    nRow--;

                    worksheet.Cells[nRowBegin, 6, nRow, 6].Style.Numberformat.Format = "_(* #,##0_);_(* (#,##0);_(* \"-\"??_);_(@_)";
                    worksheet.Cells[nRowBegin, 1, nRow, nCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    worksheet.Cells[nRowBegin, 1, nRow, nCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    worksheet.Cells[nRowBegin, 1, nRow, nCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    worksheet.Cells[nRowBegin, 1, nRow, nCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                    return ExcelHelpers.SaveFileExcel(package, Directory.GetCurrentDirectory(), "SalaryUserVersion");
                }
            }
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }
}