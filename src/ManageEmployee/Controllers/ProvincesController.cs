using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using ManageEmployee.Services.Interfaces.Addresses;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.AddressEntities;
using ManageEmployee.DataTransferObject.AddressModels;
using ManageEmployee.DataTransferObject.BaseResponseModels;

namespace ManageEmployee.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProvincesController : ControllerBase
{
    private readonly IProvinceService _provinceService;
    private readonly IMapper _mapper;

    public ProvincesController(
        IProvinceService provinceService,
        IMapper mapper)
    {
        _provinceService = provinceService;
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] PagingRequestModel param)
    {
        var provinces = _provinceService.GetAll(param.Page, param.PageSize).ToList();
        var totalItems = _provinceService.Count();
        var model = _mapper.Map<IList<ProvinceModel>>(provinces);
        return Ok(new BaseResponseModel
        {
            TotalItems = totalItems,
            Data = model,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        });
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetList()
    {
        var provinces = await _provinceService.GetAll();
        return Ok(new BaseResponseCommonModel
        {
            Data = provinces,
        });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var province = _provinceService.GetById(id);
        return Ok(province);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProvinceModel model)
    {
        // map model to entity and set id
        var province = _mapper.Map<Province>(model);

        // update user 
        await _provinceService.Create(province);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProvinceModel model)
    {
        // map model to entity and set id
        var province = _mapper.Map<Province>(model);
        province.Id = id;

        // update user 
        await _provinceService.Update(province);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _provinceService.Delete(id);
        return Ok();
    }
    /*
    [HttpGet("GetExcel")]
    public async Task<IActionResult> GetExcel()
    {
        string path_exc = @"D:\ath\data\excel\Danh sách chi tiết.xlsx";

        using (FileStream templateDocumentStream = System.IO.File.OpenRead(path_exc))
        {
            using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets["Sheet1"];
                int i = 6413;
                while (sheet.Cells[i, 1].Value != null)
                {
                    Province pro = new Province();
                    pro.Name = sheet.Cells[i, 1].Value.ToString();
                    pro.Code = sheet.Cells[i, 2].Value.ToString();
                    pro.Type = "";
                    pro.ZipCode = "";
                    _context.Provinces.Add(pro);
                    _context.SaveChanges();

                    while (sheet.Cells[i, 2].Value == pro.Code)
                    {
                        District dis = new District();
                        dis.Name = sheet.Cells[i, 3].Value.ToString();
                        dis.Code = sheet.Cells[i, 4].Value.ToString();
                        dis.ProvinceId = pro.Id;
                        dis.Type = "";
                        _context.Districts.Add(dis);
                        _context.SaveChanges();

                        while (sheet.Cells[i, 4].Value == dis.Code)
                        {
                            if (sheet.Cells[i, 5].Value != null)
                            {
                                Ward ward = new Ward();
                                ward.Name = sheet.Cells[i, 5].Value.ToString();
                                ward.Code = sheet.Cells[i, 6].Value.ToString();
                                ward.Type = sheet.Cells[i, 7].Value.ToString();
                                ward.DistrictId = dis.Id;
                                _context.Wards.Add(ward);
                                _context.SaveChanges();
                            }
                            i++;
                        }
                    }
                }
            }
        }
        return Ok();
    }
    */
}
