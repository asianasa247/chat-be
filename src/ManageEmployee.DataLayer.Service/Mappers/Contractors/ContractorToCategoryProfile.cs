using AutoMapper;
using ManageEmployee.DataTransferObject.Contractors;
using ManageEmployee.Entities.ContractorEntities;

namespace ManageEmployee.DataLayer.Service.Mappers.Contractors;

public class ContractorToCategoryProfile : Profile
{
    public ContractorToCategoryProfile()
    {
        CreateMap<ContractorToCategory, ContractorToCategoryDto>()
            .ReverseMap();
    }
}