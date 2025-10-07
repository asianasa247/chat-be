using ManageEmployee.DataTransferObject.PagingRequest;

namespace ManageEmployee.DataTransferObject.AddressModels;

public class DistrictViewModel
{
    public class GetByProvince : PagingRequestModel
    {
        public int? provinceid { get; set; }
    }
}