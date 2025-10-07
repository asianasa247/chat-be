using ManageEmployee.Controllers.Contractors;
using ManageEmployee.DataLayer.Service.Interfaces;
using ManageEmployee.DataTransferObject.Contractors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using Shouldly;
using System.Net;
using NSubstitute.ExceptionExtensions;
using ManageEmployee.DataTransferObject;
using ManageEmployee.Entities.GoodsEntities;

namespace ManageEmployeeTest;

[TestFixture]
public class ContractorControllerTests
{
    private IContractorService _contractorService;
    private ILogger<ContractorController> _logger;
    private ContractorController _controller;

    [SetUp]
    public void SetUp()
    {
        _contractorService = Substitute.For<IContractorService>();
        _logger = Substitute.For<ILogger<ContractorController>>();
        _controller = new ContractorController(_contractorService, _logger);
    }

    [Test]
    public void GetContractorsByUserId_ReturnsOkResult_WhenUserIdIsValid()
    {
        // Arrange
        int userId = 1;
        var dto = Result.Complete(new List<UserToContractorDto> 
        {
            new UserToContractorDto() 
        });

        _contractorService.GetContractorsByUserId(userId).Returns(dto);

        // Act
        var result = _controller.GetContractorsByUserId(userId);

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.ShouldBeOfType<List<UserToContractorDto>>();
        okResult!.Value.ShouldBe(dto.Data);
    }

    [Test]
    public void GetContractorsByUserId_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        int userId = 1;
        _contractorService.GetContractorsByUserId(userId).Throws(new Exception());

        // Act
        var result = _controller.GetContractorsByUserId(userId);

        // Assert
        result.ShouldBeOfType<StatusCodeResult>();
        var statusCodeResult = result as StatusCodeResult;
        statusCodeResult!.StatusCode.ShouldBe((int)HttpStatusCode.InternalServerError);
    }

    [Test]
    public void GetContractorByUserId_ReturnsOkResult_WhenUserIdIsValid()
    {
        // Arrange
        int userId = 1;
        var dto = Result.Complete(new UserToContractorDto());

        _contractorService.GetContractorByUserId(userId).Returns(dto);

        // Act
        var result = _controller.GetContractorByUserId(userId);

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.ShouldBeOfType<UserToContractorDto>();
        okResult!.Value.ShouldBe(dto.Data);
    }

    [Test]
    public void GetContractorByUserId_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        int userId = 1;
        _contractorService.GetContractorByUserId(userId).Throws(new Exception());

        // Act
        var result = _controller.GetContractorByUserId(userId);

        // Assert
        result.ShouldBeOfType<StatusCodeResult>();
        var statusCodeResult = result as StatusCodeResult;
        statusCodeResult!.StatusCode.ShouldBe((int)HttpStatusCode.InternalServerError);
    }

    [Test]
    public void GetContractorByDomain_ReturnsOkResult_WhenUserIdIsValid()
    {
        // Arrange
        string domain = "domain";
        var dto = Result.Complete(new UserToContractorDto());

        _contractorService.GetContractorByDomain(domain).Returns(dto);

        // Act
        var result = _controller.GetContractorByDomain(domain);

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.ShouldBeOfType<UserToContractorDto>();
        okResult!.Value.ShouldBe(dto.Data);
    }

    [Test]
    public void GetContractorByDomain_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        string domain = "domain";
        _contractorService.GetContractorByDomain(domain).Throws(new Exception());

        // Act
        var result = _controller.GetContractorByDomain(domain);

        // Assert
        result.ShouldBeOfType<StatusCodeResult>();
        var statusCodeResult = result as StatusCodeResult;
        statusCodeResult!.StatusCode.ShouldBe((int)HttpStatusCode.InternalServerError);
    }

    [Test]
    public void GetCategoriesByContractorDomain_ReturnsOkResult_WhenUserIdIsValid()
    {
        // Arrange
        string domain = "domain";
        var dto = Result.Complete(new List<ContractorToCategoryDto>());

        _contractorService.GetCategoriesByContractorDomain(domain).Returns(dto);

        // Act
        var result = _controller.GetCategoriesByContractorDomain(domain);

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.ShouldBeOfType<List<ContractorToCategoryDto>>();
        okResult!.Value.ShouldBe(dto.Data);
    }

    [Test]
    public void GetCategoriesByContractorDomain_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        string domain = "domain";
        _contractorService.GetCategoriesByContractorDomain(domain).Throws(new Exception());

        // Act
        var result = _controller.GetCategoriesByContractorDomain(domain);

        // Assert
        result.ShouldBeOfType<StatusCodeResult>();
        var statusCodeResult = result as StatusCodeResult;
        statusCodeResult!.StatusCode.ShouldBe((int)HttpStatusCode.InternalServerError);
    }

    [Test]
    public void GetCategoriesByContractorId_ReturnsOkResult_WhenUserIdIsValid()
    {
        // Arrange
        Guid contractId = Guid.NewGuid();
        var dto = Result.Complete(new List<ContractorToCategoryDto>());

        _contractorService.GetCategoriesByContractId(contractId).Returns(dto);

        // Act
        var result = _controller.GetCategoriesByContractorId(contractId);

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.ShouldBeOfType<List<ContractorToCategoryDto>>();
        okResult!.Value.ShouldBe(dto.Data);
    }

    [Test]
    public void GetCategoriesByContractorId_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        Guid contractId = Guid.NewGuid();
        _contractorService.GetCategoriesByContractId(contractId).Throws(new Exception());

        // Act
        var result = _controller.GetCategoriesByContractorId(contractId);

        // Assert
        result.ShouldBeOfType<StatusCodeResult>();
        var statusCodeResult = result as StatusCodeResult;
        statusCodeResult!.StatusCode.ShouldBe((int)HttpStatusCode.InternalServerError);
    }

    [Test]
    public void GetProductByContractorCategoryId_ReturnsOkResult_WhenUserIdIsValid()
    {
        // Arrange
        Guid contractId = Guid.NewGuid();
        var pageIndex = 1;
        var pageSize = 10;

        var dto = Result.Complete(new List<Goods>());

        _contractorService.GetProductsByContractorCategoryId(contractId, pageIndex, pageSize).Returns(dto);

        // Act
        var result = _controller.GetProductByContractorCategoryId(contractId, pageIndex, pageSize);

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.ShouldBeOfType<List<Goods>>();
        okResult!.Value.ShouldBe(dto.Data);
    }

    [Test]
    public void GetProductByContractorCategoryId_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        Guid contractId = Guid.NewGuid();
        var pageIndex = 1;
        var pageSize = 10;

        _contractorService.GetProductsByContractorCategoryId(contractId, pageIndex, pageSize).Throws(new Exception());

        // Act
        var result = _controller.GetProductByContractorCategoryId(contractId, pageIndex, pageSize);

        // Assert
        result.ShouldBeOfType<StatusCodeResult>();
        var statusCodeResult = result as StatusCodeResult;
        statusCodeResult!.StatusCode.ShouldBe((int)HttpStatusCode.InternalServerError);
    }

    [Test]
    public void AddContractor_ReturnsOkResult_WhenUserIdIsValid()
    {
        // Arrange
        var request = new UserToContractorDto();

        var dto = Result.Complete(new UserToContractorDto());

        _contractorService.AddContractor(request).Returns(dto);

        // Act
        var result = _controller.AddContractor(request);

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.ShouldBeOfType<UserToContractorDto>();
        okResult!.Value.ShouldBe(dto.Data);
    }

    [Test]
    public void AddContractor_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var request = new UserToContractorDto();

        _contractorService.AddContractor(request).Throws(new Exception());

        // Act
        var result = _controller.AddContractor(request);

        // Assert
        result.ShouldBeOfType<StatusCodeResult>();
        var statusCodeResult = result as StatusCodeResult;
        statusCodeResult!.StatusCode.ShouldBe((int)HttpStatusCode.InternalServerError);
    }

    [Test]
    public void AddCategoryToContractor_ReturnsOkResult_WhenUserIdIsValid()
    {
        // Arrange
        var request = new AddCategoryToContractorDto();

        var dto = Result.Complete(new ContractorToCategoryDto());

        _contractorService.AddCategoryToContractor(request).Returns(dto);

        // Act
        var result = _controller.AddCategoryToContractor(request);

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.ShouldBeOfType<ContractorToCategoryDto>();
        okResult!.Value.ShouldBe(dto.Data);
    }

    [Test]
    public void AddCategoryToContractor_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var request = new AddCategoryToContractorDto();

        _contractorService.AddCategoryToContractor(request).Throws(new Exception());

        // Act
        var result = _controller.AddCategoryToContractor(request);

        // Assert
        result.ShouldBeOfType<StatusCodeResult>();
        var statusCodeResult = result as StatusCodeResult;
        statusCodeResult!.StatusCode.ShouldBe((int)HttpStatusCode.InternalServerError);
    }

    [Test]
    public void AddProductToCategory_ReturnsOkResult_WhenUserIdIsValid()
    {
        // Arrange
        var dto = Result.Complete(new List<CategoryToProductsDto>());

        _contractorService.AddProductsToCategory(Arg.Any<AddCategoryToProductsDto>()).Returns(dto);

        // Act
        var result = _controller.AddProductToCategory(Guid.NewGuid(), new List<int> ());

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.ShouldBeOfType<List<CategoryToProductsDto>>();
        okResult!.Value.ShouldBe(dto.Data);
    }

    [Test]
    public void AddProductToCategory_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        _contractorService.AddProductsToCategory(Arg.Any<AddCategoryToProductsDto>()).Throws(new Exception());

        // Act
        var result = _controller.AddProductToCategory(Guid.NewGuid(), new List<int>());

        // Assert
        result.ShouldBeOfType<StatusCodeResult>();
        var statusCodeResult = result as StatusCodeResult;
        statusCodeResult!.StatusCode.ShouldBe((int)HttpStatusCode.InternalServerError);
    }

    [Test]
    public void GetProductsByDomain_ReturnsOkResult_WhenUserIdIsValid()
    {
        // Arrange
        var domain = "domain";
        var pageIndex = 1;
        var pageSize = 10;

        var dto = Result.Complete(new List<Goods>());

        _contractorService.GetProductByContractDomain(domain, pageIndex, pageSize).Returns(dto);

        // Act
        var result = _controller.GetProductsByDomain(domain, pageIndex, pageSize);

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.ShouldBeOfType<List<Goods>>();
        okResult!.Value.ShouldBe(dto.Data);
    }

    [Test]
    public void GetProductsByDomain_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var domain = "domain";
        var pageIndex = 1;
        var pageSize = 10;

        _contractorService.GetProductByContractDomain(domain, pageIndex, pageSize).Throws(new Exception());

        // Act
        var result = _controller.GetProductsByDomain(domain, pageIndex, pageSize);

        // Assert
        result.ShouldBeOfType<StatusCodeResult>();
        var statusCodeResult = result as StatusCodeResult;
        statusCodeResult!.StatusCode.ShouldBe((int)HttpStatusCode.InternalServerError);
    }
}
