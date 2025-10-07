using ManageEmployee.Entities.ContractorEntities;
using ManageEmployee.Entities.GoodsEntities;

namespace ManageEmployee.DataLayer.Service.Test.TestUtils;

internal class ContractorGenerator
{
    public static List<UserToContractor> GenerateContractorGenerator()
    {
        return new List<UserToContractor>
        {
            new UserToContractor
            {
                UserToContractorId = Guid.NewGuid(),
                UserId = 1,
                Domain = "domain1",
                IsDeleted = false,
            },
            new UserToContractor
            {
                UserToContractorId = Guid.NewGuid(),
                UserId = 2,
                Domain = "domain2",
                IsDeleted = false,
            },
            new UserToContractor
            {
                UserToContractorId = Guid.NewGuid(),
                UserId = 3,
                Domain = "domain3",
                IsDeleted = true,
            }
        };
    }

    public static List<ContractorToCategory> GenerateContractorToCategory(Guid userToContractorId)
    {
        return new List<ContractorToCategory>
        {
            new ContractorToCategory
            {
                ContractorToCategoryId = Guid.NewGuid(),
                UserToContractorId = userToContractorId,
                CategoryName = "CategoryName 01",
                IsDeleted = false,
                SortOrder = 1
            },
            new ContractorToCategory
            {
                ContractorToCategoryId = Guid.NewGuid(),
                UserToContractorId = userToContractorId,
                CategoryName = "CategoryName 02",
                IsDeleted = true,
                SortOrder = 2
            },
            new ContractorToCategory
            {
                ContractorToCategoryId = Guid.NewGuid(),
                UserToContractorId = Guid.NewGuid(),
                CategoryName = "CategoryName 03",
                IsDeleted = false,
                SortOrder = 2
            }
        };
    }

    public static List<ContractorToCategoryToProduct> GenerateContractorToCategoryToProduct(Guid contractToCategoryId)
    {
        return new List<ContractorToCategoryToProduct>
        {
            new ContractorToCategoryToProduct
            {
                ContractorToCategoryToProductId = Guid.NewGuid(),
                ContractToCategoryId = contractToCategoryId,
                ProductId = 1
            },
            new ContractorToCategoryToProduct
            {
                ContractorToCategoryToProductId = Guid.NewGuid(),
                ContractToCategoryId = contractToCategoryId,
                ProductId = 2
            },
            new ContractorToCategoryToProduct
            {
                ContractorToCategoryToProductId = Guid.NewGuid(),
                ContractToCategoryId = contractToCategoryId,
                ProductId = 3
            },
            new ContractorToCategoryToProduct
            {
                ContractorToCategoryToProductId = Guid.NewGuid(),
                ContractToCategoryId = Guid.NewGuid(),
                ProductId = 4
            }
        };
    }

    public static List<Goods> GenerateProducts()
    {
        return new List<Goods>
        {
            new Goods
            {
                Id = 1
            },
            new Goods
            {
                Id = 2
            },
            new Goods
            {
                Id = 3
            },
            new Goods
            {
                Id = 4
            },
        };
    }
}
