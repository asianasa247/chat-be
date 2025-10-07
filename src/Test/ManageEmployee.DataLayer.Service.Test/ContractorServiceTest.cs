using AutoMapper;
using ManageEmployee.DataLayer.Service.Test.TestUtils;
using ManageEmployee.DataLayer.Service.Test.TestUtils.DbContextBuilders;
using ManageEmployee.DataTransferObject.Contractors;
using ManageEmployee.Entities.ContractorEntities;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using NUnit.Framework;
using Shouldly;
using System.Diagnostics.Contracts;

namespace ManageEmployee.DataLayer.Service.Test;


internal class ContractorServiceTest
{
    private IMapper mapper => new Mapper(new MapperConfiguration(cfg => cfg.AddMaps(new[]
    {
        typeof(ContractorService).Assembly
    })));

    [Test]
    [TestCase("domain1")]
    [TestCase("DOMAIN1")]
    [TestCase("dOMAIN1")]
    public void GetCategoriesByContractorDomain_Domain_DoesNot_Found_ReturnValue(string domain)
    {
        var userToContractors = ContractorGenerator.GenerateContractorGenerator();

        var contractorToCategoryDbSet = ContractorGenerator.GenerateContractorToCategory(userToContractors[0]!.UserToContractorId);

        var dbContext = MockDbContext.Create()
                        .MockDbSet(userToContractors)
                        .MockDbSet(contractorToCategoryDbSet)
                        .GetDbContext();

        var service = new ContractorService(dbContext, mapper);
        var result = service.GetCategoriesByContractorDomain(domain);
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.Count.ShouldBe(1);
    }

    [Test]
    [TestCase("domain1")]
    public void GetCategoriesByContractorDomain_Category_DoesNot_Found_ReturnEmpty(string domain)
    {
        var userToContractors = ContractorGenerator.GenerateContractorGenerator();

        var contractorToCategoryDbSet = ContractorGenerator.GenerateContractorToCategory(Guid.NewGuid());

        var dbContext = MockDbContext.Create()
                        .MockDbSet(userToContractors)
                        .MockDbSet(contractorToCategoryDbSet)
                        .GetDbContext();

        var service = new ContractorService(dbContext, mapper);
        var result = service.GetCategoriesByContractorDomain(domain);
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.Count.ShouldBe(0);
    }

    [Test]
    [TestCase("domain12")]
    public void GetCategoriesByContractorDomain_Contract_Exist_ReturnFails(string domain)
    {
        var userToContractors = ContractorGenerator.GenerateContractorGenerator();

        var contractorToCategoryDbSet = ContractorGenerator.GenerateContractorToCategory(userToContractors[0]!.UserToContractorId);

        var dbContext = MockDbContext.Create()
                        .MockDbSet(userToContractors)
                        .MockDbSet(contractorToCategoryDbSet)
                        .GetDbContext();

        var service = new ContractorService(dbContext, mapper);
        var result = service.GetCategoriesByContractorDomain(domain);
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.Count.ShouldBe(0);
    }

    [Test]
    public void GetCategoriesByContractId_ReturnValue()
    {
        var userToContractors = ContractorGenerator.GenerateContractorGenerator();
        var userToContractID = userToContractors[0]!.UserToContractorId;

        var contractorToCategoryDbSet = ContractorGenerator.GenerateContractorToCategory(userToContractID);

        var dbContext = MockDbContext.Create()
                        .MockDbSet(userToContractors)
                        .MockDbSet(contractorToCategoryDbSet)
                        .GetDbContext();

        var service = new ContractorService(dbContext, mapper);
        var result = service.GetCategoriesByContractId(userToContractID);
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.Count.ShouldBe(1);
    }

    [Test]
    public void GetCategoriesByContractId_ReturnEmpty()
    {
        var userToContractors = ContractorGenerator.GenerateContractorGenerator();
        var userToContractID = Guid.NewGuid();

        var contractorToCategoryDbSet = ContractorGenerator.GenerateContractorToCategory(userToContractID);

        var dbContext = MockDbContext.Create()
                        .MockDbSet(userToContractors)
                        .MockDbSet(contractorToCategoryDbSet)
                        .GetDbContext();

        var service = new ContractorService(dbContext, mapper);
        var result = service.GetCategoriesByContractId(userToContractID);
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.Count.ShouldBe(0);
    }

    [Test]
    public void GetContractorsByUserId_ReturnValue()
    {
        var userId = 1;

        var userToContractors = ContractorGenerator.GenerateContractorGenerator();
        var userToContractID = userToContractors[0]!.UserToContractorId;

        var dbContext = MockDbContext.Create()
                        .MockDbSet(userToContractors)
                        .GetDbContext();

        var service = new ContractorService(dbContext, mapper);
        var result = service.GetContractorsByUserId(userId);
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
    }

    [Test]
    public void GetContractorsByUserId_ReturnFail()
    {
        var userId = 10;

        var userToContractors = ContractorGenerator.GenerateContractorGenerator();

        var dbContext = MockDbContext.Create()
                        .MockDbSet(userToContractors)
                        .GetDbContext();

        var service = new ContractorService(dbContext, mapper);
        var result = service.GetContractorsByUserId(userId);
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeFalse();
        result.Data.ShouldBeNull();
    }

    [Test]
    public void GetContractorByUserId_ReturnValue()
    {
        var userId = 1;

        var userToContractors = ContractorGenerator.GenerateContractorGenerator();
        var userToContractID = userToContractors[0]!.UserToContractorId;

        var dbContext = MockDbContext.Create()
                        .MockDbSet(userToContractors)
                        .GetDbContext();

        var service = new ContractorService(dbContext, mapper);
        var result = service.GetContractorByUserId(userId);
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
    }

    [Test]
    public void GetContractorByUserId_ReturnFail()
    {
        var userId = 10;

        var userToContractors = ContractorGenerator.GenerateContractorGenerator();

        var dbContext = MockDbContext.Create()
                        .MockDbSet(userToContractors)
                        .GetDbContext();

        var service = new ContractorService(dbContext, mapper);
        var result = service.GetContractorByUserId(userId);
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeFalse();
        result.Data.ShouldBeNull();
    }

    [Test]
    [TestCase("domain1")]
    [TestCase("DOMAIN1")]
    [TestCase("dOMAIN1")]
    public void GetContractorByDomain_ReturnValue(string domain)
    {
        var userToContractors = ContractorGenerator.GenerateContractorGenerator();

        var dbContext = MockDbContext.Create()
                        .MockDbSet(userToContractors)
                        .GetDbContext();

        var service = new ContractorService(dbContext, mapper);
        var result = service.GetContractorByDomain(domain);
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
    }

    [Test]
    [TestCase("domain12")]
    public void GetContractorByDomain_ReturnFail(string domain)
    {
        var userToContractors = ContractorGenerator.GenerateContractorGenerator();

        var dbContext = MockDbContext.Create()
                        .MockDbSet(userToContractors)
                        .GetDbContext();

        var service = new ContractorService(dbContext, mapper);
        var result = service.GetContractorByDomain(domain);
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeFalse();
        result.Data.ShouldBeNull();
    }

    [Test]
    public void AddContractor_ReturnValue()
    {
        var userToContractors = ContractorGenerator.GenerateContractorGenerator();

        var dbContext = MockDbContext.Create()
                        .MockDbSet(new List<UserToContractor>())
                        .GetDbContext();

        var service = new ContractorService(dbContext, mapper);

        var dto = new UserToContractorDto
        {
            UserId = 1,
            Domain = "domain 1"
        };

        var result = service.AddContractor(dto);

        dbContext.Received().SaveChanges();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.UserId.ShouldBe(dto.UserId);
        result.Data.Domain.ShouldBe(dto.Domain);
        result.Data.UserToContractorId.ShouldNotBe(Guid.Empty);
    }

    [Test]
    public void AddContractor_ReturnFails()
    {
        var userToContractors = ContractorGenerator.GenerateContractorGenerator();

        var dbContext = MockDbContext.Create()
                        .MockDbSet(new List<UserToContractor>())
                        .GetDbContext();

        dbContext.SaveChanges().Returns(0);

        var service = new ContractorService(dbContext, mapper);

        var dto = new UserToContractorDto
        {
            UserId = 1,
            Domain = "domain 1"
        };

        var result = service.AddContractor(dto);

        dbContext.Received().SaveChanges();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeFalse();
        result.Data.ShouldBeNull();
    }

    [Test]
    public void AddCategoryToContractor_ReturnValue()
    {
        var userToContractors = ContractorGenerator.GenerateContractorGenerator();

        var dbContext = MockDbContext.Create()
                        .MockDbSet(new List<ContractorToCategory>())
                        .GetDbContext();

        var service = new ContractorService(dbContext, mapper);

        var dto = new AddCategoryToContractorDto
        {
            CategoryName = "Test",
            ContractId = Guid.NewGuid(),
        };

        var result = service.AddCategoryToContractor(dto);

        dbContext.Received().SaveChanges();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.CategoryName.ShouldBe(dto.CategoryName);
        result.Data.ContractorToCategoryId.ShouldNotBe(Guid.Empty);
        result.Data.UserToContractorId.ShouldBe(dto.ContractId);
    }

    [Test]
    public void AddCategoryToContractor_ReturnFails()
    {
        var userToContractors = ContractorGenerator.GenerateContractorGenerator();

        var dbContext = MockDbContext.Create()
                        .MockDbSet(new List<UserToContractor>())
                        .GetDbContext();

        dbContext.SaveChanges().Returns(0);

        var service = new ContractorService(dbContext, mapper);

        var dto = new AddCategoryToContractorDto
        {
            CategoryName = "Test",
            ContractId = Guid.NewGuid(),
        };

        var result = service.AddCategoryToContractor(dto);

        dbContext.Received().SaveChanges();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeFalse();
        result.Data.ShouldBeNull();
    }

    [Test]
    public void AddProductsToCategory_ReturnValue()
    {
        var userToContractors = ContractorGenerator.GenerateContractorGenerator();

        var dbContext = MockDbContext.Create()
                        .MockDbSet(new List<ContractorToCategoryToProduct>())
                        .GetDbContext();

        var service = new ContractorService(dbContext, mapper);

        var dto = new AddCategoryToProductsDto
        {
           CategoryId = Guid.NewGuid(),
           ProductIds = new List<int> { 1 }
        };

        var result = service.AddProductsToCategory(dto);

        dbContext.Received().SaveChanges();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();

        result.Data[0].ContractToCategoryId.ShouldBe(dto.CategoryId);
        result.Data[0].ProductId.ShouldBe(dto.ProductIds[0]);
        result.Data[0].ContractorToCategoryToProductId.ShouldNotBe(Guid.Empty);
    }

    [Test]
    public void AddProductsToCategory_ReturnFails()
    {
        var userToContractors = ContractorGenerator.GenerateContractorGenerator();

        var dbContext = MockDbContext.Create()
                        .MockDbSet(new List<UserToContractor>())
                        .GetDbContext();

        dbContext.SaveChanges().Returns(0);

        var service = new ContractorService(dbContext, mapper);

        var dto = new AddCategoryToProductsDto
        {
            CategoryId = Guid.NewGuid(),
            ProductIds = new List<int> { 1 }
        };

        var result = service.AddProductsToCategory(dto);

        dbContext.Received().SaveChanges();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeFalse();
        result.Data.ShouldBeNull();
    }

    [Test]
    public void GetProductsByContractorCategoryId_ReturnValue()
    {
        var contractorToCategoryDbSet = ContractorGenerator.GenerateContractorToCategory(Guid.NewGuid());

        var categoryId = contractorToCategoryDbSet[0].ContractorToCategoryId;

        var contractorToCategoryToProductDbSet = ContractorGenerator.GenerateContractorToCategoryToProduct(categoryId);
        var goodsDbSet = ContractorGenerator.GenerateProducts();
       

        var dbContext = MockDbContext.Create()
                        .MockDbSet(contractorToCategoryDbSet)
                        .MockDbSet(contractorToCategoryToProductDbSet)
                        .MockDbSet(goodsDbSet)
                        .GetDbContext();

        var service = new ContractorService(dbContext, mapper);
        var result = service.GetProductsByContractorCategoryId(categoryId, 1, 10);
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.Count.ShouldBe(3);
    }

    [Test]
    public void GetProductsByContractorCategoryId_ReturnFail()
    {
        var contractorToCategoryDbSet = ContractorGenerator.GenerateContractorToCategory(Guid.NewGuid());

        var categoryId = Guid.NewGuid();

        var contractorToCategoryToProductDbSet = ContractorGenerator.GenerateContractorToCategoryToProduct(categoryId);
        var goodsDbSet = ContractorGenerator.GenerateProducts();


        var dbContext = MockDbContext.Create()
                        .MockDbSet(contractorToCategoryDbSet)
                        .MockDbSet(contractorToCategoryToProductDbSet)
                        .MockDbSet(contractorToCategoryToProductDbSet)
                        .MockDbSet(goodsDbSet)
                        .GetDbContext();

        var service = new ContractorService(dbContext, mapper);
        var result = service.GetProductsByContractorCategoryId(categoryId, 1, 10);

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldBeEmpty();
    }

    [Test]
    public void GetProductByContractDomain_ReturnValue()
    {
        var domain = "domain1";

        var userToContractoryDbSet = ContractorGenerator.GenerateContractorGenerator();

        var contractorToCategoryDbSet = ContractorGenerator.GenerateContractorToCategory(userToContractoryDbSet[0].UserToContractorId);

        var categoryId = contractorToCategoryDbSet[0].ContractorToCategoryId;

        var contractorToCategoryToProductDbSet = ContractorGenerator.GenerateContractorToCategoryToProduct(categoryId);
        var goodsDbSet = ContractorGenerator.GenerateProducts();

        var dbContext = MockDbContext.Create()
                        .MockDbSet(userToContractoryDbSet)
                        .MockDbSet(contractorToCategoryDbSet)
                        .MockDbSet(contractorToCategoryToProductDbSet)
                        .MockDbSet(goodsDbSet)
                        .GetDbContext();

        var service = new ContractorService(dbContext, mapper);
        var result = service.GetProductByContractDomain(domain, 1, 10);
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.Count.ShouldBe(3);
    }

    [Test]
    public void GetProductByContractDomain_ReturnFail()
    {
        var domain = "domain10";

        var userToContractoryDbSet = ContractorGenerator.GenerateContractorGenerator();

        var contractorToCategoryDbSet = ContractorGenerator.GenerateContractorToCategory(userToContractoryDbSet[0].UserToContractorId);

        var categoryId = contractorToCategoryDbSet[0].ContractorToCategoryId;

        var contractorToCategoryToProductDbSet = ContractorGenerator.GenerateContractorToCategoryToProduct(categoryId);
        var goodsDbSet = ContractorGenerator.GenerateProducts();

        var dbContext = MockDbContext.Create()
                        .MockDbSet(userToContractoryDbSet)
                        .MockDbSet(contractorToCategoryDbSet)
                        .MockDbSet(contractorToCategoryToProductDbSet)
                        .MockDbSet(goodsDbSet)
                        .GetDbContext();

        var service = new ContractorService(dbContext, mapper);
        var result = service.GetProductByContractDomain(domain, 1, 10);

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldBeEmpty();
    }

    [Test]
    public void UpdateContractor_Completed()
    {
        var userToContractors = ContractorGenerator.GenerateContractorGenerator();
        var userToContract= userToContractors[0]!;

        var dbContext = MockDbContext.Create()
                        .MockDbSet(userToContractors)
                        .GetDbContext();

        var service = new ContractorService(dbContext, mapper);
        var result = service.UpdateContractor(new UpdateUserToContractorDto(
                        userToContract.UserToContractorId,
                        "ContractorDomain",
                        true
                    ));

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.IsDeleted.ShouldBeTrue();
        result.Data.Domain.ShouldBe("ContractorDomain");
    }

    [Test]
    public void UpdateContractor_Fails()
    {
        var userToContractors = ContractorGenerator.GenerateContractorGenerator();
        var userToContract = userToContractors[0]!;

        var dbContext = MockDbContext.Create()
                        .MockDbSet(userToContractors)
                        .GetDbContext();

        var service = new ContractorService(dbContext, mapper);
        var result = service.UpdateContractor(new UpdateUserToContractorDto(
                        Guid.NewGuid(),
                        "ContractorDomain",
                        true
                    ));

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeFalse();
    }
}
