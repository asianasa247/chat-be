using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ManageEmployee.Filters;
using ManageEmployee.Services.Interfaces.Webs;
using ManageEmployee.DataTransferObject.Web;
using ManageEmployee.DataTransferObject.Requests;

namespace ManageEmployee.Controllers.Web;

[Authorize]
[Route("api/[controller]")]
[ApiController]
[TypeFilter(typeof(ResponseWrapperFilterAttribute))]
public class WebCartController : ControllerBase
{
    private readonly IWebCartService _webCartService;

    public WebCartController(
        IWebCartService webCartService)
    {
        _webCartService = webCartService;
    }

    [HttpGet("getCartByCustomer")]
    public async Task<IActionResult> GetCartByCustomer()
    {
        var userId = 0;

        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            userId = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
        }
        return Ok(await _webCartService.GetByCustomerId(userId));
    }

    [HttpPost("get-cart-product-info")]
    [AllowAnonymous]
    public async Task<IActionResult> GetShoppingProductCartInfo(List<ShoppingCartProductInfoRequest> cartProductsRequest)
    {
        var result = await _webCartService.GetCartProductInfoAsync(cartProductsRequest);
        return Ok(result);
    }

    [HttpPost("addProduct")]
    public async Task<IActionResult> AddProductToCart([FromBody] CartEditViewModel request)
    {
        var userId = 0;

        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            userId = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
        }
        return Ok(await _webCartService.Create(request, userId));
    }

    [HttpPost("updateCart")]
    public async Task<IActionResult> UpdateCart([FromBody] List<CartEditViewModel> requests)
    {
        var userId = 0;

        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            userId = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
        }
        foreach (var request in requests)
        {
            await _webCartService.Update(request, userId);
        }
        return Ok(new CommonWebResponse
        {
            Code = 200,
            State = true,
            Message = "",
            Data = null
        });
    }

    [HttpDelete("deleteCart/{id}")]
    public async Task<IActionResult> DeleteProductCart(int id)
    {
        var userId = 0;

        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            userId = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
        }
        await _webCartService.Delete(id, userId);
        return Ok(new CommonWebResponse
        {
            Code = 200,
            State = true,
            Message = "",
            Data = null
        });
    }

    [HttpDelete("deleteCart")]
    public async Task<IActionResult> DeleteCart()
    {
        var userId = 0;

        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            userId = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
        }
        return Ok(await _webCartService.DeleteAll(userId));
    }

    [HttpGet("countCart")]
    public async Task<IActionResult> Count()
    {
        var userId = 0;

        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            userId = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
        }
        return Ok(await _webCartService.CountCart(userId));
    }
}