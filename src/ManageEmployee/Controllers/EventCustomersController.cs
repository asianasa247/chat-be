using System.Linq;
using System.Threading.Tasks;
using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.EventModels;
using ManageEmployee.Entities.CustomerEntities;
using ManageEmployee.Services.Interfaces.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers
{
    [Authorize] // Mặc định: cần đăng nhập
    [Route("api/[controller]")]
    [ApiController]
    public class EventCustomersController : ControllerBase
    {
        private readonly IEventCustomerService _service;

        public EventCustomersController(IEventCustomerService service)
        {
            _service = service;
        }

        /// <summary>GET /api/EventCustomers (PUBLIC)</summary>
        [AllowAnonymous] // <-- Cho phép ẩn danh cho GET ALL
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponseModel), 200)]
        public async Task<IActionResult> GetAll()
        {
            // dbName: đã được FE truyền qua query (?dbName=...) và header (X-Db-Name)
            // Nếu bạn có middleware/tenant resolver thì không cần lấy ra ở đây.
            var data = await _service.GetAllAsync();
            return Ok(new BaseResponseModel
            {
                Data = data,
                TotalItems = data?.Count() ?? 0,
                CurrentPage = 1,
                PageSize = data?.Count() ?? 0
            });
        }

        /// <summary>GET /api/EventCustomers/{id} (PUBLIC - tùy, bật cho tiện)</summary>
        [AllowAnonymous] // <-- Cho phép ẩn danh cho GET BY ID (nếu muốn public)
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ObjectReturn), 200)]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return Ok(new ObjectReturn { data = item, status = 200 });
        }

        /// <summary>POST /api/EventCustomers (Require Auth)</summary>
        [HttpPost]
        [ProducesResponseType(typeof(ObjectReturn), 200)]
        public async Task<IActionResult> Create([FromBody] EventCustomerCreateDto dto)
        {
            var entity = new EventCustomer
            {
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                EventCode = dto.EventCode,
                EventName = dto.EventName,
                Supervisor = dto.Supervisor,
                Note = dto.Note
            };

            var created = await _service.CreateAsync(entity);
            return Ok(new ObjectReturn { data = created, status = 200 });
        }

        /// <summary>PUT /api/EventCustomers/{id} (Require Auth)</summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ObjectReturn), 200)]
        public async Task<IActionResult> Update(int id, [FromBody] EventCustomerUpdateDto dto)
        {
            var entity = new EventCustomer
            {
                Id = id,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                EventCode = dto.EventCode,
                EventName = dto.EventName,
                Supervisor = dto.Supervisor,
                Note = dto.Note
            };

            var updated = await _service.UpdateAsync(entity);
            return Ok(new ObjectReturn { data = updated, status = 200 });
        }

        /// <summary>DELETE /api/EventCustomers/{id} (Require Auth)</summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ObjectReturn), 200)]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok(new ObjectReturn { status = 200 });
        }
    }
}
