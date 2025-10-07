using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.AreaModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.AreaEntities;
using ManageEmployee.Services.Interfaces.NewHotels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace ManageEmployee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class AreaController : ControllerBase
    {
        private readonly IAreaService _areaService;

        public AreaController(IAreaService areaService)
        {
            _areaService = areaService;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AreaDTO model)
        {
            var area = await _areaService.Add(model);
            return Ok(area);
        }

        [HttpPut]
        [Route("{Id}")]
        public async Task<IActionResult> Edit([FromRoute] int Id, [FromBody] AreaDTO model)
        {
            var area = await _areaService.Edit(Id, model);
            return area != null ? Ok(area) : NotFound();
        }

        [HttpDelete]
        [Route("{Id}")]
        public async Task<IActionResult> Delete([FromRoute] int Id)
        {
            var result = await _areaService.Delete(Id);
            return result ? Ok() : NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var areas = await _areaService.GetAll();
            return Ok(areas);
        }

        [HttpGet]
        [Route("{Id}")]
        public async Task<IActionResult> GetById([FromRoute] int Id)
        {
            var area = await _areaService.GetById(Id);
            return area != null ? Ok(area) : NotFound();
        }

        [HttpGet]
        [Route("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] PagingRequestModel pagingRequest)
        {
            var result = await _areaService.GetPaged(pagingRequest);
            return result != null ? Ok(result) : BadRequest();
        }

        [HttpGet("export-excel")]
        public async Task<IActionResult> ExportExcel()
        {
            try
            {
                string fileName = await _areaService.ExportExcel();
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "ExportHistory", "EXCEL");
                string filePath = Path.Combine(folderPath, fileName);

                byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            var result = await _areaService.ImportExcel(file);
            return Ok(new { message = result });
        }

    }


}

