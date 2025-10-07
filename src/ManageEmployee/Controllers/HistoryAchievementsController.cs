using AutoMapper;
using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities;
using ManageEmployee.Extends;
using ManageEmployee.Services.Interfaces.Assets;
using ManageEmployee.Services.Interfaces.HistoryAchievements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class HistoryAchievementsController : ControllerBase
{
    private readonly IHistoryAchievementsService _historyAchievementsService;
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;

    public HistoryAchievementsController(
        IHistoryAchievementsService historyAchievementsService,
        IMapper mapper,
        IFileService fileService)
    {
        _historyAchievementsService = historyAchievementsService;
        _mapper = mapper;
        _fileService = fileService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] HistoryAchievementViewModel param)
    {
        var relatives = await _historyAchievementsService.GetAll(param);
        return Ok(relatives);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _historyAchievementsService.GetById(id);
        return Ok(user);
    }

    [HttpPut("{id}")]
    [HttpPost]
    public async Task<IActionResult> Update([FromForm] HistoryAchievementModel model, int id = 0)
    {
        // map model to entity and set id
        var historyAchievement = _mapper.Map<HistoryAchievement>(model);
        historyAchievement.Id = id;

        var identityUser = HttpContext.GetIdentityUser();

        historyAchievement.UserUpdated = identityUser.Id;
        historyAchievement.UserCreated = identityUser.Id;

        historyAchievement.FileUrl = "";
        if (model.File != null)
        {
            var fileName = _fileService.Upload(model.File, "HistoryAchievement");
            historyAchievement.FileUrl = fileName;
            historyAchievement.FileName = model.File.FileName;
        }

        if (historyAchievement.Id != 0)
        {
            await _historyAchievementsService.UpdateAsync(historyAchievement);
        }
        else
        {
            await _historyAchievementsService.Create(historyAchievement);
        }
        // update user
        return Ok();
    }

    [HttpPost("savelist")]
    public async Task<IActionResult> SaveListUser([FromBody] List<HistoryAchievementModel> model)
    {
        // map model to entity and set id
        if (model != null && model.Any())
        {
            foreach (var relativeModel in model)
            {
                var historyAchievement = _mapper.Map<HistoryAchievement>(relativeModel);
                var identityUser = HttpContext.GetIdentityUser();
                historyAchievement.UserUpdated = identityUser.Id;
                historyAchievement.UserCreated = identityUser.Id;
                await _historyAchievementsService.Create(historyAchievement);
                // update user
            }
        }
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _historyAchievementsService.Delete(id);
        return Ok();
    }

    //[HttpPost("export")]
    //public ActionResult Export([FromBody] List<int> ids)
    //{
    //    List<RelativeMapper.RelativeExport> datas = _relativeService.GetForExcel(string.Join(",", ids)).ToList();
    //    if (datas.Any())
    //    {
    //        string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Excel/ThongTinNhanVien.xlsx");
    //        MemoryStream stream = new MemoryStream();
    //        using (FileStream templateDocumentStream = System.IO.File.OpenRead(path))
    //        {
    //            using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
    //            {
    //                ExcelWorksheet sheet = package.Workbook.Worksheets["Sheet1"];
    //                sheet.DefaultColWidth = 10.0;
    //                int rowIdx = 11;

    //                foreach (RelativeMapper.RelativeExport lo in datas)
    //                {
    //                    sheet.Cells.Style.WrapText = true;
    //                    sheet.Cells[rowIdx, 1].Value = rowIdx - 10;
    //                    sheet.Cells[rowIdx, 4].Value = lo.FullName;
    //                    GenderEnum gioiTinh = lo.Gender;
    //                    if (gioiTinh == GenderEnum.Male)
    //                    {
    //                        sheet.Cells[rowIdx, 7].Value = "x";
    //                    }
    //                    else if (gioiTinh == GenderEnum.Female)
    //                    {
    //                        sheet.Cells[rowIdx, 8].Value = "x";
    //                    }
    //                    sheet.Cells[rowIdx, 9].Value = lo.BirthDay.HasValue ? lo.BirthDay.Value.ToString("dd") : "";
    //                    sheet.Cells[rowIdx, 10].Value = lo.BirthDay.HasValue ? lo.BirthDay.Value.ToString("MM") : "";
    //                    sheet.Cells[rowIdx, 11].Value = lo.BirthDay.HasValue ? lo.BirthDay.Value.ToString("yyyy") : "";
    //                    sheet.Cells[rowIdx, 12].Value = lo.NativePlace;
    //                    sheet.Cells[rowIdx, 13].Value = lo.PlaceOfPermanent;
    //                    sheet.Cells[rowIdx, 16].Value = lo.EthnicGroup;
    //                    sheet.Cells[rowIdx, 17].Value = lo.Religion;
    //                    sheet.Cells[rowIdx, 19].Value = lo.UnionMember ? "x" : "";
    //                    sheet.Cells[rowIdx, 20].Value = lo.Phone;
    //                    sheet.Cells[rowIdx, 22].Value = lo.Identify;
    //                    sheet.Cells[rowIdx, 23].Value = lo.IdentifyCreatedDate.HasValue ? lo.IdentifyCreatedDate.Value.ToString("dd/MM/yyyy") : "";
    //                    sheet.Cells[rowIdx, 24].Value = lo.IdentifyCreatedPlace;
    //                    sheet.Cells[rowIdx, 26].Value = lo.Literacy;
    //                    sheet.Cells[rowIdx, 27].Value = lo.Specialize;
    //                    sheet.Cells[rowIdx, 28].Value = lo.LiteracyDetail;
    //                    sheet.Cells[rowIdx, 31].Value = lo.Total;
    //                    rowIdx++;
    //                }
    //                package.SaveAs(stream);
    //            }
    //        }
    //        stream.Seek(0L, SeekOrigin.Begin);
    //        string fileName = string.Format("ThongtinNV_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmss"));
    //        return this.File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    //    }
    //    else
    //    {
    //        return BadRequest(new { msg = "Không có dữ liệu xuất file" });
    //    }
    //}

    [HttpGet("getAllActive")]
    public async Task<IActionResult> GetAllActive()
    {
        var result = await _historyAchievementsService.GetAllActive();
        return Ok(new BaseResponseModel
        {
            Data = result,
        });
    }
}