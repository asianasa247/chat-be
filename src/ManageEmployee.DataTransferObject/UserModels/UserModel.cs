using ManageEmployee.DataTransferObject.FileModels;
using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.DataTransferObject.UserModels;

public class UserModel
{
    public int Id { get; set; }
    public int? BranchId { get; set; }
    public int? WarehouseId { get; set; }
    public int? DepartmentId { get; set; }
    public int? PositionDetailId { get; set; }
    public int? TargetId { get; set; }
    public int? SymbolId { get; set; }
    public string Language { get; set; } = "";
    public string Note { get; set; } = "";
    public string? FullName { get; set; }
    public string Phone { get; set; } = "";
    public DateTime? BirthDay { get; set; }
    public GenderEnum Gender { get; set; }
    public string Email { get; set; } = "";
    public string Facebook { get; set; } = "";
    public int? ProvinceId { get; set; }
    public int? DistrictId { get; set; }
    public int? WardId { get; set; }
    public string Address { get; set; } = "";
    #region CMND
    public string Identify { get; set; } = "";
    public DateTime? IdentifyCreatedDate { get; set; }
    public string IdentifyCreatedPlace { get; set; } = "";
    public DateTime? IdentifyExpiredDate { get; set; }
    public int? NativeProvinceId { get; set; }
    public int? NativeDistrictId { get; set; }
    public int? NativeWardId { get; set; }
    public string PlaceOfPermanent { get; set; } = "";
    public string Nation { get; set; } = ""; // Quốc gia
    public string Religion { get; set; } = "";// Tôn giáo
    public string EthnicGroup { get; set; } = ""; // Dân tộc
    public int UnionMember { get; set; } = 0; // Đoàn viên
    public string LicensePlates { get; set; } = ""; // Biến số xe
    public bool isDemobilized { get; set; } = false; // xuất ngũ
    #endregion
    #region Trình độ
    public string Literacy { get; set; } = "";
    public string LiteracyDetail { get; set; } = "";
    public int? MajorId { get; set; }
    public string CertificateOther { get; set; } = "";
    #endregion
    #region lương, ngày phép
    public string BankAccount { get; set; } = "";
    public string Bank { get; set; } = "";
    public string ShareHolderCode { get; set; } = "";
    public int? NoOfLeaveDate { get; set; }
    public DateTime? SendSalaryDate { get; set; }
    public int? ContractTypeId { get; set; }
    public double Salary { get; set; } = 0;
    public double SocialInsuranceSalary { get; set; } = 0;
    public double NumberWorkdays { get; set; } = 0;
    public int? DayOff { get; set; }
    #endregion
    #region Thuế
    public string PersonalTaxCode { get; set; } = "";
    public string SocialInsuranceCode { get; set; } = "";
    public DateTime? SocialInsuranceCreated { get; set; }
    #endregion

    public string Username { get; set; } = "";
    public int Timekeeper { get; set; } = 0;
    public string Avatar { get; set; } = "";
    public string UserRoleIds { get; set; } = "";
    public bool Status { get; set; } = false;
    public DateTime? LastLogin { get; set; }
    public bool RequestPassword { get; set; } = false;
    public bool Quit { get; set; } = false;
    public string? UserRoleName { get; set; }
    public string? AddressFull { get; set; }
    public string? NativeAddressFull { get; set; }
    public string? DepartmentName { get; set; }
    public List<AllowanceUserViewModel>? ListAllowanceUser { get; set; }
    public string? ProvinceName { get; set; }
    public string? DistrictName { get; set; }
    public string? WardName { get; set; }
    public string? PassEmail { get; set; }
    public string? SignFile { get; set; }
    public int YearCurrent { get; set; } = 0;
    public string? SeriPhone { get; set; }

    public int? TimekeeperId { get; set; }
    public int? NumberOfMeals { get; set; }

    #region thu viec
    public DateTime? ProbationFromAt { get; set; }
    public DateTime? ProbationToAt { get; set; }
    public double? SalaryPercentage { get; set; }
    #endregion

    public bool IsContractor { get; set; }
    public string? ContractorDomain { get; set; }

    public List<FileDetailModel>? Images { get; set; }
}
