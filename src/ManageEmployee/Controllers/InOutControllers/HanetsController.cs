using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.InOutModels;
using ManageEmployee.Services.Interfaces.InOuts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.InOutControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HanetsController : ControllerBase
    {
        private readonly IHanetInOut _hanetAPI;
        private readonly IHanetRegister _hanetRegister;
        private readonly IHanetPlace _hanetPlace;
        public HanetsController(IHanetInOut hanetAPI, IHanetRegister hanetRegister, IHanetPlace hanetPlace)
        {
            _hanetAPI = hanetAPI;
            _hanetRegister = hanetRegister;
            _hanetPlace = hanetPlace;
        }
        [HttpPost]
        public async Task<IActionResult> SetDataLogIn(HanetModel data)
        {
            await _hanetAPI.SetDataLogIn(data);
            return Ok();
        }

        [HttpGet("place/list")]
        public async Task<IActionResult> GetListPlace()
        {
           var response =  await _hanetPlace.GetList();
            return Ok(new BaseResponseCommonModel { Data = response});
        }
    }
}
