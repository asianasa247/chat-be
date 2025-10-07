using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.GoodsModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.SearchModels;
using ManageEmployee.Entities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.GoodsEntities;
using ManageEmployee.Extends;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Assets;
using ManageEmployee.Services.Interfaces.Goods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class GoodsController : ControllerBase
{
    private readonly IGoodsService _goodsService;
    private readonly IGoodsDetailService _goodsDetailService;
    private readonly IFileService _fileService;
    private readonly IGoodPriceListService _goodPriceListService;
    private readonly IGoodSynchronizer _goodSynchronizer;
    private readonly IGoodImporter _goodImporter;
    private readonly IGoodExporter _goodExporter;
    private readonly IGoodSetter _goodSetter;
    private readonly IGoodsInWarehouseReporter _goodsInWarehouseReporter;

    public GoodsController(
        IGoodsService goodsService,
        IGoodsDetailService goodsDetailService,
        IFileService fileService,
        IGoodPriceListService goodPriceListService,
        IGoodSynchronizer goodSynchronizer,
        IGoodImporter goodImporter,
        IGoodExporter goodExporter,
        IGoodSetter goodSetter,
        IGoodsInWarehouseReporter goodsInWarehouseReporter)
    {
        _goodsService = goodsService;
        _goodsDetailService = goodsDetailService;
        _fileService = fileService;
        _goodPriceListService = goodPriceListService;
        _goodSynchronizer = goodSynchronizer;
        _goodImporter = goodImporter;
        _goodExporter = goodExporter;
        _goodSetter = goodSetter;
        _goodsInWarehouseReporter = goodsInWarehouseReporter;
    }

    [HttpGet()]
    public async Task<IActionResult> GetAll([FromQuery] SearchViewModel param, [FromHeader] int yearFilter)
    {
        var result = await _goodsService.GetPaging(param, yearFilter);
        return Ok(new BaseResponseModel
        {
            TotalItems = result.TotalItems,
            Data = result.Goods,
            PageSize = result.PageSize,
            CurrentPage = result.pageIndex
        });
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetGoods()
    {
        var goods = await _goodsService.GetAllGoodShowWeb();

        return Ok(new ObjectReturn
        {
            data = goods,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute]int id, [FromHeader]int yearFilter)
    {
        var goods = await _goodsService.GetById(id, yearFilter);
        return Ok(goods);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] GoodsUpdateModel model)
    {
        int year = DateTime.Now.Year;

        if (await _goodSetter.CheckExistGoods(model))
        {
            return Ok(new ObjectReturn
            {
                message = ErrorMessages.GoodsCodeAlreadyExist,
                status = Convert.ToInt32(ErrorEnum.GOODS_IS_EXIST)
            });
        }
        var result = await _goodSetter.Create(model, year);
        if (string.IsNullOrEmpty(result))
            return Ok(new ObjectReturn
            {
                data = result,
                status = Convert.ToInt32(ErrorEnum.SUCCESS)
            });
        return BadRequest(new { msg = result });
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] GoodsUpdateModel model, [FromHeader]int yearfilter)
    {
        if (await _goodSetter.CheckExistGoods(model))
        {
            return Ok(new ObjectReturn
            {
                message = ErrorMessages.GoodsCodeAlreadyExist,
                status = Convert.ToInt32(ErrorEnum.GOODS_IS_EXIST)
            });
        }
        var result = await _goodSetter.Update(model, yearfilter);
        if (string.IsNullOrEmpty(result))
            return Ok(new ObjectReturn
            {
                data = result,
                status = Convert.ToInt32(ErrorEnum.SUCCESS)
            }); ;
        return BadRequest(new { msg = result });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _goodSetter.Delete(id);
        return Ok();
    }

    [HttpPost("uploadImage")]
    public IActionResult UploadImage([FromForm] IFormFile file)
    {
        var fileName = _fileService.Upload(file, "Goods");
        return Ok(new { imageUrl = fileName });
    }

    [HttpDelete("deleteImages")]
    public IActionResult DeleteImages([FromBody] List<DeleteImageModel> requests)
    {
        for (int i = 0; i < requests.Count; i++)
        {
            _fileService.DeleteFileUpload(requests[i].imageUrl);
        }
        return Ok();
    }

    [HttpGet("GetDetailByGoodID/{id}")]
    public async Task<IActionResult> GetDetailByGoodID(int id)
    {
        var goodDetails = await _goodsDetailService.GetAllByGood(id);

        return Ok(new BaseResponseModel
        {
            Data = goodDetails,
        });
    }

    [HttpPost("SaveGoodDetail")]
    public async Task<IActionResult> SaveComboNorm([FromBody] List<GoodsDetailModel> model)
    {
        if (model != null && model.Any())
        {
            var listCreateNorm = new List<GoodsDetailModel>();
            foreach (var item in model)
            {
                if (item.ID != 0)
                {
                    await _goodsDetailService.Update(item);
                }
                else
                {
                    listCreateNorm.Add(item);
                }
            }
            if (listCreateNorm.Any())
            {
                await _goodsDetailService.CreateList(listCreateNorm);
            }
        }
        return Ok();
    }

    [HttpPost("DeleteGoodDetail/{id}")]
    public async Task<IActionResult> DeleteGoodDetail(int id)
    {
        await _goodsDetailService.Delete(id);
        return Ok(true);
    }

    [HttpPost]
    [Route("export-bkhh")]
    public async Task<IActionResult> Export([FromBody] SearchViewModel param, bool isManager)
    {
        var data = await _goodExporter.GetExcelReport(param, isManager);
        if (data != null)
        {
            return Ok(new BaseResponseModel
            {
                Data = data
            });
        }
        return BadRequest(new { msg = "Không tìm thấy danh sách hàng hóa thỏa yêu cầu" });
    }

    [HttpPost("import-bkhh")]
    public async Task<IActionResult> Import([FromBody] List<GoodsExportlModel> lstGoods, bool isManager)
    {
        await _goodImporter.ImportFromExcel(lstGoods, isManager);
        return Ok();
    }

    [HttpGet("SyncAccountGood")]
    public async Task<IActionResult> SyncAccountGood()
    {
        await _goodSynchronizer.SyncAccountGood(DateTime.Now.Year);
        return Ok();
    }

    [HttpPost("copy-price-list")]
    public async Task<IActionResult> CopyPriceList([FromBody] CopyPriceListRequest request)
    {
        request.UserCreated = HttpContext.GetIdentityUser().Id;
        await _goodPriceListService.CopyPriceList(request);
        return Ok();
    }

    [HttpPost("update-price-list")]
    public async Task<IActionResult> UpdatePriceList([FromHeader] int yearFilter, [FromBody] UpdatePriceListRequest request)
    {
        request.UserCreated = HttpContext.GetIdentityUser().Id;
        await _goodPriceListService.UpdatePriceList(request, yearFilter);
        return Ok();
    }

    [HttpPost("compare-good-price")]
    public async Task<IActionResult> GoodComparePrice([FromBody] ComparePriceListRequest request)
    {
        var result = await _goodPriceListService.GetPagingGoodComparePrice(request);
        return Ok(result);
    }

    [HttpPost("export-compare-good-price")]
    public IActionResult ExportGoodComparePrice([FromBody] ComparePriceListRequest request)
    {
        var result = _goodPriceListService.ExportGoodComparePrice(request);
        return Ok(new BaseResponseModel
        {
            Data = result
        });
    }

    [HttpGet("check-new-good")]
    public async Task<IActionResult> CheckGoodNew([FromHeader] int yearFilter)
    {
        var result = await _goodSynchronizer.CheckGoodNew(yearFilter);
        return Ok(new ObjectReturn
        {
            data = result
        });
    }

    [HttpGet("report-good-in-warehouse")]
    public async Task<IActionResult> ReportForGoodsInWarehouse([FromQuery] SearchViewModel param, [FromHeader] int yearFilter)
    {
        var result = await _goodsInWarehouseReporter.ReportForGoodsInWarehouse(param, yearFilter);
        return Ok(result);
    }

    [HttpPost("get-prices-by-price-code/{priceCode}")]
    public async Task<IActionResult> GetGoodPricesByPriceCode(string? priceCode, [FromBody] List<GoodCodeModel> goodCodes)
    {
        var goods = await _goodPriceListService.GetPriceByPriceCode(priceCode, goodCodes);
        return Ok(goods);
    }

    [HttpPut("update-for-website/{id}")]
    public async Task<IActionResult> UpdateGoodWebsite([FromBody] Goods model)
    {
        var result = await _goodSetter.UpdateGoodsWebsite(model);
        return Ok(new ObjectReturn
        {
            data = result,
            status = Convert.ToInt32(ErrorEnum.SUCCESS)
        });
    }

    [HttpPost("menu-type-for-goods")]
    public async Task<IActionResult> UpdateMenuTypeForGood([FromBody] UpdateMenuTypeForGoodModel model)
    {
        await _goodSetter.UpdateMenuTypeForGood(model);
        return Ok();
    }

    [HttpPost("status-goods/{status}")]
    public async Task<IActionResult> UpdateStatusGoods([FromBody] List<int> goodIds, int status)
    {
        await _goodSetter.UpdateStatusGoods(goodIds, status);
        return Ok();
    }

    [HttpPost("service-goods")]
    public async Task<IActionResult> UpdateGoodIsService([FromBody] List<int> goodIds)
    {
        await _goodSetter.UpdateGoodIsService(goodIds);
        return Ok();
    }

    [HttpGet("ExportExcel-SCT")]
    public async Task<IActionResult> ExportExcelSCT(int year)
    {
        var fileContent = await _goodsService.ExportExcelSCT(year);

        if (fileContent == null || fileContent.Length == 0)
            return NotFound("Không có dữ liệu để xuất.");

        return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"SoChiTietDSHH_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");
    }

    [HttpGet("ExportExcel-CVP")]
    public async Task<IActionResult> ExportExcelCVP(int year)
    {
        var fileContent = await _goodsService.ExportExcelCVP(year);

        if (fileContent == null || fileContent.Length == 0)
            return NotFound("Không có dữ liệu để xuất.");

        return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"ChiTietDSHH_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");
    }

    [HttpGet("convert-product-stock-data-paginator")]
    public async Task<IActionResult> GetConvertProductStockDataPaginator([FromQuery] int year, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var data = await _goodsService.GetConvertProductStockDataPaginator(year, page, pageSize);
        return Ok(new BaseResponseModel
        {
            Data = data
        });
    }

    [HttpGet("convert-product-stock-data")]
    public async Task<IActionResult> GetConvertProductStockData([FromHeader] int yearFilter)
    {
        var data = await _goodsService.GetConvertProductStockData(yearFilter);
        return Ok(new BaseResponseModel
        {
            Data = data
        });
    }
    [HttpGet("convert-product-stock-data-filter")]
    public async Task<IActionResult> GetConvertProductStockDataFilter(
        [FromQuery] int year,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] string? warehouse,
        [FromQuery] string? productName)
    {
        var data = await _goodsService.GetConvertProductStockDataFilter(year, fromDate, toDate, warehouse, productName);
        return Ok(new BaseResponseModel
        {
            Data = data
        });
    }
}