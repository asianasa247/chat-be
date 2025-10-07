using AutoMapper;
using ManageEmployee.DataTransferObject.Contractors;
using ManageEmployee.Entities.ContractorEntities;

namespace ManageEmployee.DataLayer.Service.Mappers.Contractors;

public class ContractorToCategoryToProductProfile : Profile
{
    public ContractorToCategoryToProductProfile()
    {
        CreateMap<ContractorToCategoryToProduct, AddCategoryToProductsDto>()
            .ReverseMap();

        CreateMap<ContractorToCategoryToProduct, CategoryToProductsDto>()
           .ReverseMap();
    }
}