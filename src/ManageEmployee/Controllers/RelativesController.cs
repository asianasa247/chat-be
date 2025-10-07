using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using System.Security.Claims;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Excels;
using ManageEmployee.Services.Interfaces.Assets;
using ManageEmployee.Services.Interfaces.Relatives;
using ManageEmployee.DataTransferObject;
using ManageEmployee.Entities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.Constants;
using ManageEmployee.DataTransferObject.BaseResponseModels;

namespace ManageEmployee.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RelativesController : ControllerBase
{
    private readonly IRelativeService _relativeService;
    private readonly IMapper _mapper;
    private readonly AppSettings _appSettings;
    private readonly IFileService _fileService;
    private readonly IExcelService _excelService;

    public RelativesController(
        IRelativeService relativeService,
        IMapper mapper,
        IOptions<AppSettings> appSettings,
        IFileService fileService, IExcelService excelService)
    {
        _relativeService = relativeService;
        _mapper = mapper;
        _appSettings = appSettings.Value;
        _fileService = fileService;
        _excelService = excelService;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] RelativeMapper.FilterParams param)
    {
        var relatives = _relativeService.Filter(param);
        var totalItems = _relativeService.CountFilter(param);
        return Ok(new BaseResponseModel
        {
            CurrentPage = param.Page,
            PageSize = param.PageSize,
            Data = relatives,
            DataTotal = totalItems,
            TotalItems = totalItems.Count()
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _relativeService.GetById(id);
        return Ok(user);
    }

    [HttpPut("{id}")]
    [HttpPost]
    public IActionResult Update([FromBody] Relative relative, int id = 0)
    {
        // map model to entity and set id
        relative.Id = id;

        try
        {
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                relative.UserUpdated = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
                relative.UserCreated = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
            }

            if (relative.Id != 0)
            {
                var res = _relativeService.Update(relative);
                return Ok(res);
            }
            else
            {
                var res = _relativeService.Create(relative);
                return Ok(res);
            }
        }
        catch (ErrorException ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpPost("savelist")]
    public IActionResult SaveListUser([FromBody] List<Relative> model)
    {
        // map model to entity and set id
        if (model != null && model.Any())
        {
            try
            {
                foreach (var relative in model)
                {
                    if (HttpContext.User.Identity is ClaimsIdentity identity)
                    {
                        relative.UserUpdated = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
                        relative.UserCreated = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
                    }
                    var res = _relativeService.Create(relative);
                    // update user
                }
            }
            catch (ErrorException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { msg = ex.Message });
            }
        }
        return Ok();
    }

    [HttpPut("saveWithAvatar/{id}")]
    [HttpPut("saveWithAvatar")]
    public IActionResult UpdateWithAvatar(int id, [FromForm] RelativeViewModel model)
    {
        // map model to entity and set id
        var relative = _mapper.Map<Relative>(model);
        relative.Id = id;

        try
        {
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                relative.UserUpdated = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
                relative.UserCreated = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
            }

            if (relative.Id != 0)
            {
                if (model.AvataFile != null)
                {
                    if (!string.IsNullOrEmpty(relative.Avatar))
                    {
                        _fileService.DeleteFileUpload(relative.Avatar);
                    }
                    var fileName = _fileService.Upload(model.AvataFile, "user_avatar");
                    relative.Avatar = fileName;
                }
                var res = _relativeService.Update(relative);
                return Ok(res);
            }
            else
            {
                if (model.AvataFile != null)
                {
                    var fileName = _fileService.Upload(model.AvataFile, "user_avatar");
                    relative.Avatar = fileName;
                }
                var res = _relativeService.Create(relative);
                return Ok(res);
            }
            // update user
        }
        catch (ErrorException ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _relativeService.Delete(id);
        return Ok();
    }

    [HttpPut("uploadAvatar/{id}")]
    public async Task<IActionResult> UploadAvatar(int id, [FromForm] IFormFile file)
    {
        try
        {
            var user = await _relativeService.GetById(id);
            if (user != null && !user.IsDelete)
            {
                if (!string.IsNullOrEmpty(user.Avatar))
                {
                    _fileService.DeleteFileUpload(user.Avatar);
                }
                var fileName = _fileService.Upload(file, "user_avatar");
                user.Avatar = fileName;
                _relativeService.Update(user);
                return Ok(new
                {
                    id = id,
                    Avatar = fileName
                }); ;
            }
            else
            {
                return BadRequest(new { msg = ResultErrorConstants.USER_EMPTY_OR_DELETE });
            }
        }
        catch (System.Exception ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpPost("export")]
    public async Task<IActionResult> Export()
    {
        var datas = await _relativeService.GetForExcel();
        if (datas.Any())
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Excel/ThongTinNhanVien.xlsx");
            MemoryStream stream = new MemoryStream();
            using (FileStream templateDocumentStream = System.IO.File.OpenRead(path))
            {
                using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
                {
                    ExcelWorksheet sheet = package.Workbook.Worksheets["Sheet1"];
                    sheet.DefaultColWidth = 10.0;
                    int rowIdx = 11;
                    int provineColIndex = 14;
                    int districtColIndex = 15;
                    int wardColIndex = 16;
                    await _excelService.PrepareLocationRawSheetDataExcel(package, sheet, rowIdx, datas.Count(),provineColIndex, districtColIndex, wardColIndex);
                    
                    foreach (Relative lo in datas)
                    {
                        sheet.Cells.Style.WrapText = true;
                        sheet.Cells[rowIdx, 1].Value = rowIdx - 10;
                        sheet.Cells[rowIdx, 5].Value = lo.FullName;
                        GenderEnum gioiTinh = lo.Gender;
                        if (gioiTinh == GenderEnum.Male)
                        {
                            sheet.Cells[rowIdx, 8].Value = "x";
                        }
                        else if (gioiTinh == GenderEnum.Female)
                        {
                            sheet.Cells[rowIdx, 9].Value = "x";
                        }
                        sheet.Cells[rowIdx, 10].Value = lo.BirthDay.HasValue ? lo.BirthDay.Value.ToString("dd") : "";
                        sheet.Cells[rowIdx, 11].Value = lo.BirthDay.HasValue ? lo.BirthDay.Value.ToString("MM") : "";
                        sheet.Cells[rowIdx, 12].Value = lo.BirthDay.HasValue ? lo.BirthDay.Value.ToString("yyyy") : "";
                        sheet.Cells[rowIdx, 13].Value = lo.NativePlace;
                        sheet.Cells[rowIdx, provineColIndex].Value = "";
                        sheet.Cells[rowIdx, districtColIndex].Value = "";
                        sheet.Cells[rowIdx, wardColIndex].Value = "";
                        sheet.Cells[rowIdx, 17].Value = lo.PlaceOfPermanent;
                        sheet.Cells[rowIdx, 18].Value = lo.EthnicGroup;
                        sheet.Cells[rowIdx, 19].Value = lo.Religion;
                        sheet.Cells[rowIdx, 21].Value = lo.UnionMember;
                        sheet.Cells[rowIdx, 22].Value = lo.Phone;
                        sheet.Cells[rowIdx, 24].Value = lo.Identify;
                        sheet.Cells[rowIdx, 25].Value = lo.IdentifyCreatedDate.HasValue ? lo.IdentifyCreatedDate.Value.ToString("dd/MM/yyyy") : "";
                        sheet.Cells[rowIdx, 26].Value = lo.IdentifyCreatedPlace;
                        sheet.Cells[rowIdx, 27].Value = lo.Literacy;
                        sheet.Cells[rowIdx, 28].Value = lo.MajorId;
                        //sheet.Cells[rowIdx, 28].Value = lo.LiteracyDetail;
                        sheet.Cells[rowIdx, 31].Value = lo.Total;
                        rowIdx++;
                    }
                    package.SaveAs(stream);
                }
            }
            stream.Seek(0L, SeekOrigin.Begin);
            string fileName = string.Format("ThongtinNV_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmss"));
            return this.File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        else
        {
            return BadRequest(new { msg = "Không có dữ liệu xuất file" });
        }
    }

    [HttpGet("getAllUserActive")]
    public async Task<IActionResult> GetAllUserActive()
    {
        var result = await _relativeService.GetAllUserActive();
        return Ok(new BaseResponseModel
        {
            Data = result,
        });
    }

    [HttpPost("Export-relationship")]
    public async Task<IActionResult> ExportRelationShip(int userId)
    {
        var result = await _relativeService.ExportRelationShip(userId);
        return Ok(new BaseResponseModel
        {
            Data = result,
        });
    }
}