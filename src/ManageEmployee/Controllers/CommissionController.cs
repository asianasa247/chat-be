using ManageEmployee.DataTransferObject;
using ManageEmployee.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class CommissionController : ControllerBase
{
    private readonly ICommissionService _commissionService;

    public CommissionController(ICommissionService commissionService)
    {
        _commissionService = commissionService;
    }

    /// <summary>
    /// Lấy danh sách tất cả Commission
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _commissionService.GetAllAsync();
        return Ok(result);
    }

    /// <summary>
    /// Lấy Commission theo ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _commissionService.GetByIdAsync(id);
        if (result == null) return NotFound(new { message = "Commission not found" });
        return Ok(result);
    }

    /// <summary>
    /// Tạo mới hoặc cập nhật Commission theo Code
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AddOrUpdate([FromBody] CommissionModels model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _commissionService.AddOrUpdateAsync(model);
        return Ok(result);
    }

    /// <summary>
    /// Cập nhật Commission theo ID
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CommissionModels model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _commissionService.UpdateAsync(id, model);
        if (result == null) return NotFound(new { message = "Commission not found" });
        return Ok(result);
    }

    /// <summary>
    /// Xóa Commission theo ID
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _commissionService.DeleteAsync(id);
        return NoContent();
    }
}
