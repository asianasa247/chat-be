using AutoMapper;
using ManageEmployee.DataTransferObject.Contractors;
using ManageEmployee.Entities.ContractorEntities;

namespace ManageEmployee.DataLayer.Service.Mappers.Contractors;

public class UserToContractorProfile : Profile
{
    public UserToContractorProfile()
    {
        CreateMap<UserToContractor, UserToContractorDto>()
            .ReverseMap();
    }
}