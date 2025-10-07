using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ManageEmployee.Helpers;
using ManageEmployee.Dal.DbContexts;
using OfficeOpenXml;
using ManageEmployee.Services.Interfaces.Relatives;
using ManageEmployee.DataTransferObject;
using ManageEmployee.Entities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.Constants;

namespace ManageEmployee.Services;

public class RelativeService : IRelativeService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public RelativeService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Relative>> GetAllUserActive()
    {
        return await _context.Relatives.Where(x => !x.IsDelete).ToListAsync();
    }


    public IEnumerable<Relative> Filter(RelativeMapper.FilterParams param)
    {
        string condition = "WHERE us.IsDelete = 0 ";
        if (!string.IsNullOrEmpty(param.SearchText))
        {
            condition += string.Format("AND (us.FullName like N'%{0}%' " +
                                                "or us.Identify like N'%{0}%'" +
                                                "or us.Email like N'%{0}%'" +
                                                "or us.Phone like N'%{0}%' )", param.SearchText);
        }

        if (param.Quit != null && param.Quit.Value)
        {
            condition += string.Format("AND us.Quit = {0} ", 1);
        }
        if (!string.IsNullOrEmpty(param.Degree))
        {
            condition += string.Format("AND us.Degree like N'%{0}%'", param.Degree);
        }
        if (!string.IsNullOrEmpty(param.CertificateOther))
        {
            condition += string.Format("AND us.CertificateOther like N'%{0}%'", param.CertificateOther);
        }
        if (param.Gender != GenderEnum.All)
        {
            condition += string.Format("AND us.Gender = {0} ", (short)param.Gender);
        }

        if (param.StartDate != null && param.EndDate != null)
        {
            condition += string.Format("AND BirthDay IS NULL OR (us.BirthDay >= '{0}' AND us.BirthDay <= '{1}') ",
                param.StartDate?.ToString("MM/dd/yyyy"),
                param.EndDate?.ToString("MM/dd/yyyy"));
        }
        else if (param.StartDate != null)
        {
            condition += string.Format("AND BirthDay IS NULL OR us.BirthDay >= '{0}'", param.StartDate?.ToString("MM/dd/yyyy"));
        }
        else if (param.EndDate != null)
        {
            condition += string.Format("AND BirthDay IS NULL OR us.BirthDay <= '{0}' ", param.EndDate?.ToString("MM/dd/yyyy"));
        }

        string query = string.Format("SELECT * FROM Relatives us {0} ", condition);
        return _context.Relatives.FromSqlRaw(query).Skip(param.PageSize * (param.Page == 0 ? param.Page : (param.Page - 1)))
            .Take(param.PageSize);
    }

    public IEnumerable<Relative> CountFilter(RelativeMapper.FilterParams param)
    {
        string condition = "WHERE us.IsDelete = 0 ";
        if (!string.IsNullOrEmpty(param.SearchText))
        {
            condition += string.Format("AND (us.FullName like N'%{0}%' " +
                                                "or us.Identify like N'%{0}%'" +
                                                "or us.Email like N'%{0}%'" +
                                                "or us.Phone like N'%{0}%' )", param.SearchText);
        }

        if (!string.IsNullOrEmpty(param.Degree))
        {
            condition += string.Format("AND us.Degree like N'%{0}%'", param.Degree);
        }
        if (!string.IsNullOrEmpty(param.CertificateOther))
        {
            condition += string.Format("AND us.CertificateOther like N'%{0}%'", param.CertificateOther);
        }
        if (param.Gender != GenderEnum.All)
        {
            condition += string.Format("AND us.Gender = {0} ", (short)param.Gender);
        }

        if (param.BirthDay != null)
        {
            condition += string.Format("AND us.BirthDay = '{0}' ", param.BirthDay);
        }

        if (param.StartDate != null && param.EndDate != null)
        {
            condition += string.Format("AND BirthDay IS NULL OR (us.BirthDay >= '{0}' AND us.BirthDay <= '{1}') ",
                param.StartDate?.ToString("MM/dd/yyyy"),
                param.EndDate?.ToString("MM/dd/yyyy"));
        }
        else if (param.StartDate != null)
        {
            condition += string.Format("AND BirthDay IS NULL OR us.BirthDay >= '{0}'", param.StartDate?.ToString("MM/dd/yyyy"));
        }
        else if (param.EndDate != null)
        {
            condition += string.Format("AND BirthDay IS NULL OR us.BirthDay <= '{0}' ", param.EndDate?.ToString("MM/dd/yyyy"));
        }

        string query = string.Format("SELECT * FROM Relatives us {0} ", condition);
        return _context.Relatives.FromSqlRaw(query);
    }

    public async Task<Relative> GetById(int id)
    {
        return await _context.Relatives.FindAsync(id);
    }

    public Relative Create(Relative relative)
    {
        // validation
        try
        {
            _context.Relatives.Add(relative);
            _context.SaveChanges();

            return relative;
        }
        catch
        {
            throw;
        }
    }

    public Relative Update(Relative relativeParam)
    {
        var relative = _context.Relatives.AsNoTracking().FirstOrDefault(x => x.Id == relativeParam.Id);
        var submitRelative = new Relative();
        if (relative == null)
            throw new ErrorException(ResultErrorConstants.USER_EMPTY_OR_DELETE);

        if (!string.IsNullOrWhiteSpace(relativeParam.FullName))
            relative.FullName = relativeParam.FullName;

        submitRelative = _mapper.Map<Relative>(relativeParam);
        submitRelative.Id = relativeParam.Id;
        submitRelative.Avatar = relativeParam.Avatar != null && relativeParam.Avatar.Length > 0 ? relativeParam.Avatar : relative.Avatar;
        submitRelative.UpdatedAt = DateTime.Now;
        submitRelative.UserUpdated = relativeParam.UserUpdated;
        submitRelative.Quit = relativeParam.Quit;

        _context.Relatives.Update(submitRelative);
        _context.SaveChanges();
        return submitRelative;
    }

    public async Task Delete(int id)
    {
        var user = await _context.Relatives.FindAsync(id);
        if (user != null)
        {
            user.IsDelete = true;
            _context.Relatives.Update(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Relative>> GetForExcel()
    {
        var datas = await _context.Relatives.Where(x => !x.IsDelete).ToListAsync();
        return datas;
    }

    public async Task<string> ExportRelationShip(int userId)
    {
        var relationShips = await _context.RelationShips.Where(x => x.EmployeeId == userId && x.Type == 2).ToListAsync();
        var personOppositeIds = relationShips.Select(x => x.PersonOppositeId).ToList();
        var relatives = await _context.Relatives.Where(x => personOppositeIds.Contains(x.Id)).ToListAsync();
        var user = await _context.Users.FindAsync(userId);

            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Excel/RelativeRelationShipTemplate.xlsx");
            MemoryStream stream = new MemoryStream();
        using (FileStream templateDocumentStream = System.IO.File.OpenRead(path))
        {
            using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets["Sheet1"];
                sheet.DefaultColWidth = 10.0;
                int rowIdx = 18;
                var relativeHaveIdentify = relatives.Where(x => !string.IsNullOrEmpty(x.Identify)).ToList();
                int i = 0;
                foreach (Relative lo in relativeHaveIdentify)
                {
                    i++;
                    sheet.Cells[rowIdx, 1].Value = i;
                    sheet.Cells[rowIdx, 2].Value = lo.FullName;
                    sheet.Cells[rowIdx, 3].Value = "";
                    sheet.Cells[rowIdx, 4].Value = lo.BirthDay?.ToString("dd/MM/yyyy");
                    sheet.Cells[rowIdx, 5].Value = lo.Nation;
                    sheet.Cells[rowIdx, 6].Value = "";
                    sheet.Cells[rowIdx, 7].Value = lo.Identify;
                    sheet.Cells[rowIdx, 8].Value = lo.IdentifyCreatedDate.HasValue ? lo.IdentifyCreatedDate.Value.ToString("dd/MM/yyyy") : "";
                    sheet.Cells[rowIdx, 9].Value = lo.IdentifyCreatedPlace;
                    sheet.Cells[rowIdx, 10].Value = lo.PlaceOfPermanent;
                    sheet.Cells[rowIdx, 11].Value = "";
                    sheet.Cells[rowIdx, 12].Value = "";
                    sheet.Cells[rowIdx, 13].Value = "";
                    sheet.Cells[rowIdx, 14].Value = lo.Address;
                    sheet.Cells[rowIdx, 15].Value = "";
                    sheet.Cells[rowIdx, 16].Value = "";
                    sheet.Cells[rowIdx, 17].Value = "";

                    var relationShip = relationShips.FirstOrDefault(x => x.PersonOppositeId == lo.Id);
                    sheet.Cells[rowIdx, 18].Value = relationShip.ClaimingYourself;
                    sheet.Cells[rowIdx, 19].Value = user.FullName;
                    sheet.Cells[rowIdx, 20].Value = user.PersonalTaxCode;

                    sheet.InsertRow(rowIdx, 1);
                    rowIdx++;
                }

                rowIdx = rowIdx + 4;
                i = 0;
                var relativeNotHaveIdentify = relatives.Where(x => string.IsNullOrEmpty(x.Identify)).ToList();
                foreach (Relative lo in relativeNotHaveIdentify)
                {
                    i++;
                    sheet.Cells[rowIdx, 1].Value = i;
                    sheet.Cells[rowIdx, 2].Value = lo.FullName;
                    sheet.Cells[rowIdx, 3].Value = "";
                    sheet.Cells[rowIdx, 4].Value = lo.BirthDay?.ToString("dd/MM/yyyy");
                    sheet.Cells[rowIdx, 5].Value = lo.Identify;
                    sheet.Cells[rowIdx, 6].Value = lo.IdentifyCreatedDate.HasValue ? lo.IdentifyCreatedDate.Value.ToString("dd/MM/yyyy") : "";
                    sheet.Cells[rowIdx, 7].Value = lo.Nation;
                    sheet.Cells[rowIdx, 7].Value = "";
                    sheet.Cells[rowIdx, 7, rowIdx, 8].Merge = true;
                    sheet.Cells[rowIdx, 9].Value = lo.IdentifyCreatedPlace;
                    sheet.Cells[rowIdx, 9, rowIdx, 10].Merge = true;
                    sheet.Cells[rowIdx, 11].Value = "";
                    sheet.Cells[rowIdx, 12].Value = "";
                    sheet.Cells[rowIdx, 13].Value = lo.NativePlace;
                    sheet.Cells[rowIdx, 13, rowIdx, 14].Merge = true;

                    var relationShip = relationShips.FirstOrDefault(x => x.PersonOppositeId == lo.Id);
                    sheet.Cells[rowIdx, 15].Value = relationShip.ClaimingYourself;
                    sheet.Cells[rowIdx, 15, rowIdx, 16].Merge = true;
                    sheet.Cells[rowIdx, 17].Value = user.FullName;
                    sheet.Cells[rowIdx, 17, rowIdx, 18].Merge = true;
                    sheet.Cells[rowIdx, 19].Value = user.PersonalTaxCode;
                    sheet.Cells[rowIdx, 19, rowIdx, 20].Merge = true;

                    sheet.InsertRow(rowIdx, 1);
                    rowIdx++;
                }
                package.SaveAs(stream);
                return ExcelHelpers.SaveFileExcel(package, Directory.GetCurrentDirectory(), "Relatives");
            }
        }
    }
}