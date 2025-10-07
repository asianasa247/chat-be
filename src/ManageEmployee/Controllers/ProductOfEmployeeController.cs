using ManageEmployee.DataTransferObject;
using ManageEmployee.Entities;
using ManageEmployee.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManageEmployee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductOfEmployeeController : ControllerBase
    {
        private readonly IProductOfEmployeeService _productService;

        public ProductOfEmployeeController(IProductOfEmployeeService productService)
        {
            _productService = productService;
        }

        // GET: api/ProductOfEmployee
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductOfEmployee>>> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products); // Trả về 200 OK với danh sách sản phẩm
        }

        // GET: api/ProductOfEmployee/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductOfEmployee>> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound(); // Trả về 404 Not Found nếu không có sản phẩm

            return Ok(product);
        }

        // POST: api/ProductOfEmployee
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] ProductOfEmployeeModels productModel)
        {
            if (productModel == null)
                return BadRequest("Invalid data."); // Trả về 400 Bad Request nếu dữ liệu không hợp lệ

            var result = await _productService.AddOrUpdateAsync(productModel);
            return Ok(result);
        }

        // PUT: api/ProductOfEmployee/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] ProductOfEmployeeModels productModel)
        {
            if (productModel == null)
                return BadRequest("Invalid data.");

            var existingProduct = await _productService.GetByIdAsync(id);
            if (existingProduct == null)
                return NotFound(); // Trả về 404 nếu không tìm thấy sản phẩm

            await _productService.UpdateAsync(id, productModel);
            return NoContent(); // Trả về 204 No Content khi cập nhật thành công
        }

        // DELETE: api/ProductOfEmployee/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existingProduct = await _productService.GetByIdAsync(id);
            if (existingProduct == null)
                return NotFound();

            await _productService.DeleteAsync(id);
            return NoContent(); // Trả về 204 khi xóa thành công
        }
        // PUT: api/ProductOfEmployee/5
        [HttpGet("by-employee-id/{id}")]
        public async Task<ActionResult> GetProductByEmployeeId(int id)
        {
            var existingProduct = await _productService.GetProductByEmployeeIdAsync(id);
            if (existingProduct == null)
                return NotFound();
            return Ok(existingProduct);
        }

        [HttpGet("by-commission-id/{employeeId}/{id}")]
        public async Task<ActionResult> GetProductByCommissionId(int id , int employeeId)
        {
            var existingProduct = await _productService.GetProductByCommIdAsync(id, employeeId);
            if (existingProduct == null)
                return NotFound();
            return Ok(existingProduct);
        }

        [HttpGet("by-good-id/{id}")]
        public async Task<ActionResult> GetProductByGoodId(int id)
        {
            var existingProduct = await _productService.GetProductByGoodIdAsync(id);
            if (existingProduct == null)
                return NotFound();
            return Ok(existingProduct);
        }
        [HttpGet("by-good-id-and-employee-id/{id}/{empId}/{commissionId}")]
        public async Task<ActionResult> GetProductByGoodId(int id, int empId,int commissionId)
        {
            var existingProduct = await _productService.GetProductByGoodIdAndEmployeeIdAsync(id,empId,commissionId);
            if (existingProduct == null)
                return NotFound();
            return Ok(existingProduct);
        }
    }
}
