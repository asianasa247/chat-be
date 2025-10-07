using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.CarModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Services.Interfaces.Assets;
using ManageEmployee.Services.Interfaces.Cars;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.CarControllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CarsController : ControllerBase
{
    private readonly ICarService _carService;
    private readonly IFileService _fileService;

    public CarsController(ICarService carService, IFileService fileService)
    {
        _carService = carService;
        _fileService = fileService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
    {
        var result = await _carService.GetPaging(param);
        return Ok(result);
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _carService.GetList();
        return Ok(new BaseResponseCommonModel
        {
            Data = result,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _carService.GetById(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CarModel model)
    {
        await _carService.Create(model);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] CarModel model)
    {
        await _carService.Update(model);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _carService.Delete(id);
        return Ok();
    }

    [HttpGet("car-field-setup")]
    public async Task<IActionResult> GetCarFieldSetup(int carId)
    {
        var result = await _carService.GetCarFieldSetup(carId);
        return Ok(result);
    }

    [HttpPut("car-field-setup")]
    public async Task<IActionResult> GetCarFieldSetup([FromQuery] int carId, [FromBody] List<CarFieldSetupModel> carFieldSetups)
    {
       await _carService.UpdateCarFieldSetup(carId, carFieldSetups);
        return Ok();
    }

    [HttpPost("uploadfile")]
    public IActionResult Uploadfile([FromForm] IFormFile file)
    {
        var response = _fileService.UploadFile(file, "Car", file.FileName);
        return Ok(response);
    }

}