using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.DataTransferObject.UserModels;

public class UserMapper
{
    public class Auth
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Username { get; set; }
        public string? Avatar { get; set; }
        public bool Status { get; set; }
        public int PositionId { get; set; }
        public DateTime? LastLogin { get; set; }
        public string? UserRoleIds { get; set; }
        public int CompanyId { get; set; }
        public int Timekeeper { get; set; } = 0;
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public string? Language { get; set; }
        public int TargetId { get; set; }
    }
    public class FilterParams
    {
        public string? Keyword { get; set; }
        public int? WarehouseId { get; set; }
        public int? PositionId { get; set; }
        public int? DepartmentId { get; set; }
        public bool? RequestPassword { get; set; }
        public bool? Quit { get; set; }
        public GenderEnum Gender { get; set; }
        public DateTime? BirthDay { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TargetId { get; set; }
        public int? Month { get; set; }
        public int DegreeId { get; set; }
        public int CertificateId { get; set; }
        public bool isSort { get; set; } = false;
        public string? SortField { get; set; }
        public List<int>? Ids { get; set; }
        public List<string>? roles { get; set; }
        public int UserId { get; set; }
    }
}
