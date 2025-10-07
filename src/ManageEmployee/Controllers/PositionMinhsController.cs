using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ManageEmployee.Services.Interfaces;
using ManageEmployee.DataTransferObject.PagingResultModels;
using AutoMapper;
using ManageEmployee.Services;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.Helpers;
namespace ManageEmployee.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PositionMinhsController : ControllerBase
    {
        private IPositionMinhService _positionMinhService;
        private IMapper _mapper;
        public PositionMinhsController(IPositionMinhService positionMinhService, IMapper mapper)
        {
            _positionMinhService = positionMinhService;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
        {
            return Ok(await _positionMinhService.GetAll(param.Page, param.PageSize, param.SearchText));
        }
        [HttpGet("List")]
        public IActionResult GetSelectList()
        {
            var positionMinh = _positionMinhService.GetAll();
            return Ok(new BaseResponseModel
            {
                Data = positionMinh,
            });

        }
        [HttpGet("id")]
        public IActionResult GetId(int id)
        {
            var model = _positionMinhService.GetByID(id);
            return Ok(model);
        }
        [HttpPost]
        public IActionResult Create([FromBody] Entities.PositionMinhs model)
        {
            //map model to entry and set Id
            var positionMinh = _mapper.Map<Entities.PositionMinhs>(model);
            try
            {
                _positionMinhService.Create(positionMinh);
                return Ok();
            }
            catch (ErrorException ex)
            {
                return Ok(new {code=400,msg=ex.Message});
            }
        }
        [HttpPut("id")]
        public IActionResult Save([FromBody] Entities.PositionMinhs model)
        {
            var positionMinh = _mapper.Map<Entities.PositionMinhs>(model);
            try
            {
                _positionMinhService.Update(positionMinh);
                return Ok();
            }
            catch (ErrorException ex)
            {
                return Ok(new { code = 400, msg = ex.Message });
            }
        }
        [HttpDelete("id")]
        public IActionResult Delete(int id)
        {
            _positionMinhService.Delete(id);
            return Ok();
        }

    }
}
