using ManageEmployee.DataLayer.Service.Interfaces;
using ManageEmployee.DataTransferObject.Contractors;
using ManageEmployee.Extends;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using ManageEmployee.Constants;
using Microsoft.AspNetCore.Mvc;
using ManageEmployee.Entities.GoodsEntities;

namespace ManageEmployee.Controllers.Contractors;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ContractorController : ControllerBase
{
    private readonly IContractorService _contractorService;
    private readonly ILogger<ContractorController> _logger;
    public ContractorController(IContractorService contractorService, ILogger<ContractorController> logger)
    {
        _contractorService = contractorService;
        _logger = logger;
    }

    [HttpGet("GetContractorsByUserId/{userId}")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(List<UserToContractorDto>), (int)HttpStatusCode.OK)]
    public IActionResult GetContractorsByUserId(int userId)
    {
        try
        {
            var result = _contractorService.GetContractorsByUserId(userId);
            return result.Return();
        }
        catch (Exception e)
        {
            _logger.LogError(e,  string.Format(HttpErrorMessage.InternalServerError, nameof(GetContractorsByUserId)));

            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }
    }

    [HttpGet("GetContractorByUserId/{userId}")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(UserToContractorDto), (int)HttpStatusCode.OK)]
    public IActionResult GetContractorByUserId(int userId)
    {
        try
        {
            var result = _contractorService.GetContractorByUserId(userId);
            return result.Return();
        }
        catch (Exception e)
        {
            _logger.LogError(e, string.Format(HttpErrorMessage.InternalServerError, nameof(GetContractorByUserId)));

            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }
    }

    [HttpGet("GetContractorByDomain/{domain}")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(UserToContractorDto), (int)HttpStatusCode.OK)]
    [AllowAnonymous]
    public IActionResult GetContractorByDomain(string? domain)
    {
        try
        {
            var result = _contractorService.GetContractorByDomain(domain);
            return result.Return();
        }
        catch (Exception e)
        {
            _logger.LogError(e, string.Format(HttpErrorMessage.InternalServerError, nameof(GetContractorByDomain)));

            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }
    }

    [HttpGet("categoriesByDomain/{domain}")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(List<ContractorToCategoryDto>), (int)HttpStatusCode.OK)]
    [AllowAnonymous]
    public IActionResult GetCategoriesByContractorDomain([FromRoute] string domain)
    {
        try
        {
            var result = _contractorService.GetCategoriesByContractorDomain(domain);
            return result.Return();
        }
        catch (Exception e)
        {
            _logger.LogError(e, string.Format(HttpErrorMessage.InternalServerError, nameof(GetCategoriesByContractorDomain)));

            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }
    }

    [HttpGet("{contractId}/categories")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(List<ContractorToCategoryDto>), (int)HttpStatusCode.OK)]
    [AllowAnonymous]
    public IActionResult GetCategoriesByContractorId([FromRoute] Guid contractId)
    {
        try
        {
            var result = _contractorService.GetCategoriesByContractId(contractId);
            return result.Return();
        }
        catch (Exception e)
        {
            _logger.LogError(e, string.Format(HttpErrorMessage.InternalServerError, nameof(GetCategoriesByContractorId)));

            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }
    }

    [HttpGet("{categoryId}/products")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(List<Goods>), (int)HttpStatusCode.OK)]
    [AllowAnonymous]
    public IActionResult GetProductByContractorCategoryId([FromRoute] Guid categoryId, [FromQuery] int pageIndex, [FromQuery] int pageSize)
    {
        try
        {
            var result = _contractorService.GetProductsByContractorCategoryId(categoryId, pageIndex, pageSize);
            return Ok(!result.IsSuccess ? new List<Goods>() : result.Data);
        }
        catch (Exception e)
        {
            _logger.LogError(e, string.Format(HttpErrorMessage.InternalServerError, nameof(GetProductByContractorCategoryId)));

            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(UserToContractorDto), (int)HttpStatusCode.OK)]
    public IActionResult AddContractor([FromBody] UserToContractorDto dto)
    {
        try
        {
            var result = _contractorService.AddContractor(dto);
            return result.Return();
        }
        catch (Exception e)
        {
            _logger.LogError(e, string.Format(HttpErrorMessage.InternalServerError, nameof(AddContractor)));

            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }
    }

    [HttpPost("category")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ContractorToCategoryDto), (int)HttpStatusCode.OK)]
    public IActionResult AddCategoryToContractor([FromBody] AddCategoryToContractorDto dto)
    {
        try
        {
            var result = _contractorService.AddCategoryToContractor(dto);
            return result.Return();
        }
        catch (Exception e)
        {
            _logger.LogError(e, string.Format(HttpErrorMessage.InternalServerError, nameof(AddCategoryToContractor)));

            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }
    }

    [HttpPost("{categoryId}/products")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(List<CategoryToProductsDto>), (int)HttpStatusCode.OK)]
    public IActionResult AddProductToCategory([FromRoute]Guid categoryId, [FromBody] List<int> productIds)
    {
        try
        {
            var result = _contractorService.AddProductsToCategory(
                new AddCategoryToProductsDto
                {
                    CategoryId = categoryId,
                    ProductIds = productIds
                }
            );
            return result.Return();
        }
        catch (Exception e)
        {
            _logger.LogError(e, string.Format(HttpErrorMessage.InternalServerError, nameof(AddProductToCategory)));

            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }
    }

    [HttpGet("productsByDomain/{domain}")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(List<Goods>), (int)HttpStatusCode.OK)]
    [AllowAnonymous]
    public IActionResult GetProductsByDomain([FromRoute]string domain, [FromQuery] int pageIndex, [FromQuery] int pageSize)
    {
        try
        {
            var result = _contractorService.GetProductByContractDomain(domain, pageIndex, pageSize);

            return result.ReturnOk();
        }
        catch (Exception e)
        {
            _logger.LogError(e, string.Format(HttpErrorMessage.InternalServerError, nameof(GetProductsByDomain)));

            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }
    }
}