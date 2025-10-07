using System.Threading.Tasks;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.EventModels;
using ManageEmployee.Services.Interfaces.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers
{
    [Authorize] // có thể AllowAnonymous cho GET nếu public
    [Route("api/[controller]")]
    [ApiController]
    public class EventRegistrationsController : ControllerBase
    {
        private readonly IEventRegistrationService _service;

        public EventRegistrationsController(IEventRegistrationService service)
        {
            _service = service;
        }

        /// <summary>
        /// Check phone tồn tại hay chưa (không side-effect)
        /// </summary>
        [AllowAnonymous]
        [HttpGet("exists")]
        [ProducesResponseType(typeof(BaseResponseModel), 200)]
        public async Task<IActionResult> Exists([FromQuery] string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return BadRequest(new { message = "phone is required" });

            var (exists, customerId) = await _service.CheckPhoneAsync(phone.Trim());
            return Ok(new BaseResponseModel
            {
                Data = new { exists, customerId },
                TotalItems = 1,
                CurrentPage = 1,
                PageSize = 1
            });
        }

        /// <summary>
        /// Đăng ký tham gia sự kiện (BẮT BUỘC eventId).
        /// Ghi 2 bảng: Customers & CustomerTaxInformations; gắn Customers.EventCustomerId = eventId.
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(typeof(BaseResponseModel), 200)]
        public async Task<IActionResult> Register([FromBody] EventRegistrationRequest dto)
        {
            var result = await _service.RegisterAsync(dto);

            return Ok(new BaseResponseModel
            {
                Data = result,
                TotalItems = 1,
                CurrentPage = 1,
                PageSize = 1
            });
        }

        /// <summary>Get all registrations (có phân trang)</summary>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponseModel), 200)]
        public async Task<IActionResult> GetAll([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
        {
            var (items, total) = await _service.GetAllAsync(pageIndex, pageSize);
            return Ok(new BaseResponseModel
            {
                Data = items,
                TotalItems = total,
                CurrentPage = pageIndex,
                PageSize = pageSize
            });
        }

        /// <summary>Get registrations by EventId (có phân trang)</summary>
        [HttpGet("event/{eventId:int}")]
        [ProducesResponseType(typeof(BaseResponseModel), 200)]
        public async Task<IActionResult> GetByEvent(int eventId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
        {
            var (items, total) = await _service.GetByEventAsync(eventId, pageIndex, pageSize);
            return Ok(new BaseResponseModel
            {
                Data = items,
                TotalItems = total,
                CurrentPage = pageIndex,
                PageSize = pageSize
            });
        }

        /// <summary>Get registration by CustomerId (1 item; 404 nếu chưa gắn event)</summary>
        [HttpGet("customer/{customerId:int}")]
        [ProducesResponseType(typeof(BaseResponseModel), 200)]
        public async Task<IActionResult> GetByCustomer(int customerId)
        {
            var data = await _service.GetByCustomerAsync(customerId);
            if (data == null) return NotFound();

            return Ok(new BaseResponseModel
            {
                Data = data,
                TotalItems = 1,
                CurrentPage = 1,
                PageSize = 1
            });
        }
    }
}
