using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataLayer.Service.Interfaces;
using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.Contractors;
using ManageEmployee.Entities.ContractorEntities;
using ManageEmployee.Entities.GoodsEntities;
using System.Diagnostics;

namespace ManageEmployee.DataLayer.Service;

public class ContractorService : IContractorService
{
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _dbContext;
    public ContractorService(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public Result<List<ContractorToCategoryDto>> GetCategoriesByContractorDomain(string domain)
    {
        var contract = GetContractorByDomain(_dbContext, domain);

        if (contract is null)
        {
            return Result.Complete(new List<ContractorToCategoryDto>());
        }

        var contractorToCategories = GetContractToCategoryByContractId(_dbContext, contract.UserToContractorId).ToList();

        var categories = _mapper.Map<List<ContractorToCategory>, List<ContractorToCategoryDto>>(contractorToCategories);

        return Result.Complete(categories);
    }

    public Result<List<ContractorToCategoryDto>> GetCategoriesByContractId(Guid contractId)
    {
        var contract = _dbContext.UserToContractor
            .FirstOrDefault(x => x.UserToContractorId == contractId);

        if (contract is null)
        {
            return Result.Complete(new List<ContractorToCategoryDto>());
        }

        var contractorToCategories = GetContractToCategoryByContractId(_dbContext, contract.UserToContractorId).ToList();

        var categories = _mapper.Map<List<ContractorToCategory>, List<ContractorToCategoryDto>>(contractorToCategories);

        return Result.Complete(categories);
    }

    public Result<List<UserToContractorDto>> GetContractorsByUserId(int userId)
    {
        var contractors = _dbContext.UserToContractor
            .Where(x => x.UserId == userId)
            .ToList();

        if (!contractors.Any())
        {
            return Result.Failed<List<UserToContractorDto>>();
        }
        var contractorDtos = _mapper.Map<List<UserToContractor>, List<UserToContractorDto>>(contractors);

        return Result.Complete(contractorDtos);
    }

    public Result<UserToContractorDto> GetContractorByUserId(int userId)
    {
        var contractors = _dbContext.UserToContractor
            .FirstOrDefault(x => x.UserId == userId);

        if (contractors is null)
        {
            return Result.Failed<UserToContractorDto>();
        }
        var contractorDtos = _mapper.Map<UserToContractor, UserToContractorDto>(contractors);

        return Result.Complete(contractorDtos);
    }

    public Result<UserToContractorDto> GetContractorByDomain(string domain)
    {
        var contractor = GetContractorByDomain(_dbContext, domain);

        if (contractor is null)
        {
            return Result.Failed<UserToContractorDto>();
        }

        var contractorDto = _mapper.Map<UserToContractor, UserToContractorDto>(contractor);
        return Result.Complete(contractorDto);
    }

    public Result<UserToContractorDto> AddContractor(UserToContractorDto dto)
    {
        try
        {
            var entity = new UserToContractor
            {
                UserToContractorId = Guid.NewGuid(),
                UserId = dto.UserId,
                Domain = dto.Domain,
                IsDeleted = dto.IsDeleted,
            };

            _dbContext.UserToContractor.Add(entity);

            var result = _dbContext.SaveChanges();
            if (result == 0)
            {
                return Result.Failed<UserToContractorDto>();
            }

            var contractorDto = _mapper.Map<UserToContractor, UserToContractorDto>(entity);

            return Result.Complete(contractorDto);
        }
        catch (Exception ex)
        {
            Debug.Write(ex);
            return Result.Failed<UserToContractorDto>();
        }
    }

    public Result<List<CategoryToProductsDto>> AddProductsToCategory(AddCategoryToProductsDto dto)
    {
        var entities = dto.ProductIds.Select(productId => new ContractorToCategoryToProduct
        {
            ContractorToCategoryToProductId = Guid.NewGuid(),
            ContractToCategoryId = dto.CategoryId,
            ProductId = productId
        }).ToList();

        _dbContext.ContractorToCategoryToProduct.AddRange(entities);
        var result = _dbContext.SaveChanges();

        if (result == 0)
        {
            return Result.Failed<List<CategoryToProductsDto>>();
        }

        var dtos = _mapper.Map<List<ContractorToCategoryToProduct>, List<CategoryToProductsDto>>(entities);

        return Result.Complete(dtos);
    }

    public Result<ContractorToCategoryDto> AddCategoryToContractor(AddCategoryToContractorDto dto)
    {
        var entity = new ContractorToCategory
        {
            CategoryName = dto.CategoryName,
            ContractorToCategoryId = Guid.NewGuid(),
            UserToContractorId = dto.ContractId,
            IsDeleted = false
        };

        _dbContext.ContractorToCategory.Add(entity);
        var result = _dbContext.SaveChanges();

        if (result == 0)
        {
            return Result.Failed<ContractorToCategoryDto>();
        }

        var contractorToCategoryDto = _mapper.Map<ContractorToCategory, ContractorToCategoryDto>(entity);

        return Result.Complete(contractorToCategoryDto);
    }

    public Result<List<Goods>> GetProductsByContractorCategoryId(Guid categoryId, int pageIndex, int pageSize)
    {
        var category = GetContractToCategoryByCategoryId(_dbContext, categoryId).FirstOrDefault();

        if (category is null)
        {
            return Result.Complete(new List<Goods>());
        }

        var products = (
            from g in _dbContext.Goods
            join ccp in _dbContext.ContractorToCategoryToProduct on g.Id equals ccp.ProductId
            where ccp.ContractToCategoryId == categoryId
            select g
        ).Skip(pageIndex - 1).Take(pageSize).ToList();

        return Result.Complete(products);
    }

    public Result<List<Goods>> GetProductByContractDomain(string domain, int pageIndex, int pageSize)
    {
        var contractor = GetContractorByDomain(_dbContext, domain);

        if (contractor is null)
        {
            return Result.Complete(new List<Goods>());
        }

        var products = (
            from g in _dbContext.Goods
            join ccp in _dbContext.ContractorToCategoryToProduct on g.Id equals ccp.ProductId
            join cc in _dbContext.ContractorToCategory on ccp.ContractToCategoryId equals cc.ContractorToCategoryId
            where cc.UserToContractorId == contractor.UserToContractorId
            select g
        ).Skip(pageIndex - 1).Take(pageSize).ToList();

        return Result.Complete(products);
    }

    private static UserToContractor? GetContractorByDomain(ApplicationDbContext context, string domain)
    {
        return context.UserToContractor
            .FirstOrDefault(x =>
                x.Domain.Equals(domain, StringComparison.CurrentCultureIgnoreCase)
                && x.IsDeleted != true
            );
    }

    private static IQueryable<ContractorToCategory> GetContractToCategoryByContractId(ApplicationDbContext context, Guid userToContractorId)
    {
        return context.ContractorToCategory
            .Where(x =>
                x.UserToContractorId == userToContractorId
                && x.IsDeleted != true
            )
            .OrderBy(x => x.SortOrder);
    }

    private static IQueryable<ContractorToCategory> GetContractToCategoryByCategoryId(ApplicationDbContext context, Guid categoryId)
    {
        return context.ContractorToCategory
            .Where(x =>
                x.ContractorToCategoryId == categoryId
                && x.IsDeleted != true
            )
            .OrderBy(x => x.SortOrder);
    }

    public Result<UserToContractorDto> UpdateContractor(UpdateUserToContractorDto dto)
    {
        try
        {
            var contractor = _dbContext
                .UserToContractor
                .FirstOrDefault(x => x.UserToContractorId == dto.UserToContractorId);

            if (contractor is null)
            {
                return Result.Failed<UserToContractorDto>();
            }

            contractor.Domain = dto.Domain;
            contractor.IsDeleted = dto.IsDeleted;

            _dbContext.SaveChanges();

            return Result.Complete(_mapper.Map<UserToContractor, UserToContractorDto>(contractor));
        }
        catch (Exception ex)
        {
            Debug.Write(ex);
            return Result.Failed<UserToContractorDto>();
        }
    }
}