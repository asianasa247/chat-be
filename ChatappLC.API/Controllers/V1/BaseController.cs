using Microsoft.AspNetCore.Mvc;

namespace ChatappLC.API.Controllers.V1;

[ApiController]
[Route("[controller]")]
public abstract class BaseController : ControllerBase
{
    protected IActionResult HandleResponse<T>(ResponseDTO<T> response) => response.Flag ? Ok(response) : BadRequest(response);
}
