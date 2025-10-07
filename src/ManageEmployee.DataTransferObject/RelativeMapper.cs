using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.DataTransferObject;

public class RelativeMapper
{
    public class FilterParams : PagingRequestModel
    {
        public bool? Quit { get; set; }
        public GenderEnum Gender { get; set; }
        public DateTime? BirthDay { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? CertificateOther { get; set; }
        public string? Degree { get; set; }
    }
}