using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.Web;
using ManageEmployee.Extends;
using ManageEmployee.Models;
using ManageEmployee.Services;
using ManageEmployee.Services.GoodsServices;
using ManageEmployee.Services.Interfaces.Categories;
using ManageEmployee.Services.Interfaces.Goods;
using ManageEmployee.Services.Interfaces.Webs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.Web;

[Route("api/[controller]")]
[ApiController]
public class WebProductController : ControllerBase
{
    private readonly IGoodsService _goodsService;
    private readonly IWebProductService _webProductService;
    private readonly ICategoryStatusWebPeriodService _categoryStatusWebPeriodService;
    private readonly IGoodsDetailService _goodsDetailService;
    private readonly IWebNewsService _webNewsService;
    private readonly ICategoryService _categoryService;
    private readonly IGoodCustomerService _goodCustomerService;
    public WebProductController(
        IGoodsService goodsService,
        IGoodsDetailService goodsDetailService,
        IWebProductService webProductService,
          IWebNewsService webNewsService,
        ICategoryStatusWebPeriodService categoryStatusWebPeriodService,
         ICategoryService categoryService
,
         IGoodCustomerService goodCustomerService)
    {
        _goodsService = goodsService;
        _goodsDetailService = goodsDetailService;
        _webProductService = webProductService;
        _categoryStatusWebPeriodService = categoryStatusWebPeriodService;
        _webNewsService = webNewsService;
        _categoryService = categoryService;
        _goodCustomerService = goodCustomerService;
    }

    /// <summary>
    /// Danh sách sản phẩm giảm giá hôm nay`
    /// </summary>
    /// <returns></returns>
    [HttpGet("getDealToday")]
    public async Task<IActionResult> GetDealToday()
    {
        var promotion = await _categoryStatusWebPeriodService.GetDealsOfDay();

        return Ok(new CommonWebResponse()
        {
            State = true,
            Code = 200,
            Message = "",
            Data = promotion
        });
    }

    [HttpGet("getAllProduct")]
    public async Task<IActionResult> GetAllProduct(int pageSize)
    {
        var results = await _goodsService.GetAll(x => x.PriceList == "BGC", pageSize);
        return Ok(new CommonWebResponse()
        {
            State = true,
            Code = 200,
            Message = "",
            Data = results
        });
    }

    [HttpPost("getProducts")]
    public async Task<IActionResult> GetProduct(ProductSearchModel search)
    {
        return Ok(await _webProductService.GetProduct(search));
    }

    /// <summary>
    /// Danh sách sản mới nhất
    /// </summary>
    /// <returns></returns>
    [HttpGet("getSampleCategory")]
    public async Task<IActionResult> GetSampleCategory(string? code)
    {
        var results = await _goodsService.GetAll(x => x.GoodsType == code);

        return Ok(new CommonWebResponse()
        {
            State = true,
            Code = 200,
            Message = "",
            Data = results
        });
    }

    /// <summary>
    /// Top 10 sản phẩm bán chạy
    /// </summary>
    /// <returns></returns>
    [HttpGet("get-top-sell")]
    public async Task<IActionResult> GetTopSell()
    {
        var results = await _webProductService.GetTopProductSell();
        return Ok(new CommonWebResponse()
        {
            State = true,
            Code = 200,
            Message = "",
            Data = results
        });
    }

    [HttpGet("get-products-by-menu")]
    public async Task<IActionResult> GetProductByMenu(string? menuType)
    {
        var results = await _webProductService.GetProductsByMenuTypeAsyncV2(menuType);
        return Ok(new CommonWebResponse()
        {
            State = true,
            Code = 200,
            Message = "",
            Data = results
        });
    }

    [HttpGet("get-products-pagging")]
    public async Task<IActionResult> GetProductPagging(int pageNum=0, int pageSize=10,string q="")
    {
        var results = await _webProductService.GetProductsPagging(pageNum,pageSize,q);
        return Ok(new CommonWebResponse()
        {
            State = true,
            Code = 200,
            Message = "",
            Data = results
        });
    }

    [HttpGet("menu/{name}")]
    public async Task<IActionResult> GetProductsByMenuTypeAndAddition(string name)
    {
        var products = await _webProductService.GetProductsByMenuTypeAndAdditionAsync(name);
        if (products == null || !products.Any())
        {
            return NotFound(new { message = "No products found." });
        }

        return Ok(products);
    }
    [HttpGet("menuWeb/{name}")]
    public async Task<IActionResult> GetProductsByWebUrlAndAddition(string name)
    {
        var products = await _webProductService.GetProductsByMenuWebAsync(name);
        if (products == null || !products.Any())
        {
            return NotFound(new { message = "No products found." });
        }

        return Ok(products);
    }

    [HttpPost("get-items-by-menu")]
    public async Task<IActionResult> GetItemsByMenu(ItemByMenuRequest data)
    {
        List<ItemByMenuResponse> responses = new List<ItemByMenuResponse>();
        if (data.MenuTypes != null)
        {
            foreach (var item in data.MenuTypes)
            {
                var newItem = new ItemByMenuResponse();
                newItem.Category = item;
                var results = await _webProductService.GetProductsByMenuTypeAsync(item.Code);
                if(results == null|| results.Count == 0)
                {
                    newItem.IsProduct = false;
                    var news = await _webNewsService.GetByCategory(item.Id);
                    var newsItm = news.Select(t => new ItemNews()
                    {
                        Id = t.Id,
                        Images = t.Images,
                        Title = t.Title,
                        TitleEnglish = t.TitleEnglish,
                        TitleKorean = t.TitleKorean,
                        CreateAt =t.CreateAt,
                        Author = t.Author,
                        PublishDate = t.PublishDate
                    }).ToList();
                    newItem.News.AddRange(newsItm);
                }
                else
                {
                    newItem.Products.AddRange(results);
                }
                responses.Add(newItem);
            }
        }
       
        return Ok(new CommonWebResponse()
        {
            State = true,
            Code = 200,
            Message = "",
            Data = responses
        });
    }
    [HttpGet("get-items-by-category-id")]
    public async Task<IActionResult> GetItemsByCategoryId(int Id)
    {
        List<ItemByMenuResponse> responses = new List<ItemByMenuResponse>();
        var category =await _categoryService.GetById(Id);
        if (category != null)
        {
            var newItem = new ItemByMenuResponse();
            newItem.Category = new ItemByMenuCategory()
            {
                 Id =Id,
                 Code =category.Code,
                 Name =category.Name,
                 NameEnglish =category.NameEnglish,
                 NameKorea =category.NameKorea,
                 Icon = category.Icon,
            }; ;
            var results = await _webProductService.GetProductsByMenuTypeAsync(category.Code);
            if (results == null || results.Count == 0)
            {
                newItem.IsProduct = false;
                var news = await _webNewsService.GetByCategory(Id);
                var newsItm = news.Select(t => new ItemNews()
                {
                    Id = t.Id,
                    Images = t.Images,
                    Title = t.Title,
                    TitleEnglish = t.TitleEnglish,
                    TitleKorean = t.TitleKorean,
                    CreateAt = t.CreateAt
                }).ToList();
                newItem.News.AddRange(newsItm);
            }
            else
            {
                newItem.Products.AddRange(results);
            }
            responses.Add(newItem);
        }

        return Ok(new CommonWebResponse()
        {
            State = true,
            Code = 200,
            Message = "",
            Data = responses
        });
    }

    [HttpGet("get-products-by-menu-paging")]
    public async Task<IActionResult> GetProductByMenu(string? menuType,[FromQuery] PagingRequestModel param, bool isService)
    {
        var results = await _webProductService.GetProductsByMenuTypeAsync(menuType, param, isService);
        return Ok(results);
    }

    /// <summary>
    /// Chi tiet san pham
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("getById/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var objResult = new WebProductDetailViewModel();
        var good = await _webProductService.GetByIdAsync(id);
        objResult.Good = good;
        objResult.Category = await _webProductService.GetCategoryByCodeAsync(good.MenuType);
        objResult.Images = new List<string>();
        if (!string.IsNullOrEmpty(objResult.Good.Image1))
        {
            objResult.Images.Add(objResult.Good.Image1);
        }

        if (!string.IsNullOrEmpty(objResult.Good.Image2))
        {
            objResult.Images.Add(objResult.Good.Image2);
        }

        if (!string.IsNullOrEmpty(objResult.Good.Image3))
        {
            objResult.Images.Add(objResult.Good.Image3);
        }

        if (!string.IsNullOrEmpty(objResult.Good.Image4))
        {
            objResult.Images.Add(objResult.Good.Image4);
        }

        if (!string.IsNullOrEmpty(objResult.Good.Image5))
        {
            objResult.Images.Add(objResult.Good.Image5);
        }

        objResult.Details = await _goodsDetailService.GetAllByGood(id);
        if (objResult.Good != null)
        {
            return Ok(new CommonWebResponse()
            {
                State = true,
                Code = 200,
                Message = "",
                Data = objResult
            });
        }
        else
        {
            return Ok(new CommonWebResponse()
            {
                State = false,
                Code = 200,
                Message = "",
                Data = objResult
            });
        }
    }

    /// <summary>
    /// Danh sách sản phẩm theo danh mục
    /// </summary>
    /// <returns></returns>
    [HttpGet("getProductCategory")]
    public async Task<IActionResult> GetProductCategory()
    {
        var results = await _webProductService.GetProductCategory();
        return Ok(new CommonWebResponse()
        {
            State = true,
            Code = 200,
            Message = "",
            Data = results
        });
    }

    /// <summary>
    ///  Get sản phẩm theo mã menuWeb
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    [HttpGet("get-good-show-web")]
    public async Task<IActionResult> GetGoodShowWeb(string? code)
    {
        var result = await _categoryStatusWebPeriodService.GetGoodShowWeb(code);
        return Ok(new ObjectReturn()
        {
            data = result,
        });
    }

    [HttpGet("good-favourite")]
    [Authorize]
    public async Task<IActionResult> GetGoodForCustomer([FromQuery] PagingRequestModel param)
    {
        var result = await _goodCustomerService.GetGoodForCustomer(param, HttpContext.GetIdentityUser().Id);
        return Ok(new ObjectReturn()
        {
            data = result,
        });
    }

    [HttpPost("good-favourite")]
    [Authorize]
    public async Task<IActionResult> GetGoodForCustomer(int goodId)
    {
        await _goodCustomerService.AddAsync(goodId, HttpContext.GetIdentityUser().Id);
        return Ok();
    }
    [HttpDelete("good-favourite")]
    [Authorize]
    public async Task<IActionResult> RemoveAsync(int goodId)
    {
        await _goodCustomerService.RemoveAsync(goodId, HttpContext.GetIdentityUser().Id);
        return Ok();
    }
}