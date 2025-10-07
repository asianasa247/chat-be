using Emgu.CV.Ocl;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Services.Interfaces.Customers;
using ManageEmployee.Services.Interfaces.ListCustomers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Controllers.GetListCustomerControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListCustomerController : ControllerBase
    {
        private readonly IListCustomerService _customerService;

        public ListCustomerController(IListCustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet("ExportCustomer/{customerId}")]
        public async Task<IActionResult> GetCustomerContactHistories(int customerId)
        {
            var fileContent = await _customerService.GetCustomerContactHistories(customerId);

            if (fileContent == null || fileContent.Length == 0)
                return NotFound("Không có dữ liệu để xuất.");

            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"DanhSachCongViec_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");
        }

    }



}

        




