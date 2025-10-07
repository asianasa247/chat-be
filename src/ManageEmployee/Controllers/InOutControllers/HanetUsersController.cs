using ManageEmployee.DataTransferObject.InOutModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Models;
using ManageEmployee.Services.Interfaces.InOuts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.InOutControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HanetUsersController : ControllerBase
    {
        private readonly IHanetUserService _hanetUserService;

        public HanetUsersController(IHanetUserService hanetUserService)
        {
            _hanetUserService = hanetUserService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(Response<PagingResult<HanetUserModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaging([FromQuery] PagingRequestModel param)
        {
            var result = await _hanetUserService.GetPaging(param);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Response<HanetUserModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _hanetUserService.GetDetail(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] HanetUserModel model)
        {
            await _hanetUserService.Set(model);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] HanetUserModel model, int id)
        {
            model.Id = id;
            await _hanetUserService.Set(model);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _hanetUserService.Delete(id);
            return Ok();
        }
    }
}