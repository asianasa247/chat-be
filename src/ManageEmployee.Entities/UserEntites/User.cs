using System.ComponentModel.DataAnnotations;
using ManageEmployee.Entities.BaseEntities;
using ManageEmployee.Entities.ContractorEntities;
using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.Entities.UserEntites;

public class User : BaseEntity
{
    public int Id { get; set; }
    public int? BranchId { get; set; }
    public int? WarehouseId { get; set; }
    public int? DepartmentId { get; set; }
    public int? PositionDetailId { get; set; }
    public int? TargetId { get; set; }
    public int? SymbolId { get; set; }
    [StringLength(36)]
    public string Language { get; set; } = "";
    [StringLength(255)]
    public string Note { get; set; } = "";
    [StringLength(255)]
    public string? FullName { get; set; }
    [StringLength(20)]
    public string? Phone { get; set; }
    public DateTime? BirthDay { get; set; }
    public GenderEnum Gender { get; set; }
    [StringLength(255)]
    public string? Email { get; set; }
    [StringLength(255)]
    public string? PassEmail { get; set; }
    [StringLength(500)]
    public string? SignFile { get; set; }

    [StringLength(255)]
    public string Facebook { get; set; } = "";
    public int? ProvinceId { get; set; }
    public int? DistrictId { get; set; }
    public int? WardId { get; set; }
    [StringLength(255)]
    public string Address { get; set; } = "";
    public string Images { get; set; } = "";
    #region CMND
    [StringLength(36)]
    public string Identify { get; set; } = "";
    public DateTime? IdentifyCreatedDate { get; set; }
    [StringLength(36)]
    public string IdentifyCreatedPlace { get; set; } = "";
    public DateTime? IdentifyExpiredDate { get; set; }
    public int? NativeProvinceId { get; set; }
    public int? NativeDistrictId { get; set; }
    public int? NativeWardId { get; set; }
    [StringLength(36)]
    public string PlaceOfPermanent { get; set; } = "";
    [StringLength(36)]
    public string Nation { get; set; } = ""; // Quốc gia
    [StringLength(36)]
    public string Religion { get; set; } = "";// Tôn giáo
    [StringLength(36)]
    public string EthnicGroup { get; set; } = ""; // Dân tộc
    public int? UnionMember { get; set; } = 0; // Đoàn viên
    [StringLength(36)]
    public string LicensePlates { get; set; } = ""; // Biến số xe
    public bool isDemobilized { get; set; } = false; // xuất ngũ
    #endregion
    #region Trình độ
    [StringLength(36)]
    public string Literacy { get; set; } = "";
    [StringLength(36)]
    public string LiteracyDetail { get; set; } = "";
    public int? MajorId { get; set; }
    [StringLength(36)]
    public string CertificateOther { get; set; } = ""; // 
    #endregion
    #region lương, ngày phép
    [StringLength(36)]
    public string BankAccount { get; set; } = "";
    [StringLength(36)]
    public string Bank { get; set; } = "";
    [StringLength(36)]
    public string ShareHolderCode { get; set; } = "";
    public int? NoOfLeaveDate { get; set; }
    public DateTime? SendSalaryDate { get; set; }
    public int? ContractTypeId { get; set; }
    public double? Salary { get; set; } = 0;
    public double? SocialInsuranceSalary { get; set; } = 0;
    public double? NumberWorkdays { get; set; } = 0;
    public int? DayOff { get; set; }
    #endregion
    #region Thuế
    [StringLength(36)]
    public string PersonalTaxCode { get; set; } = "";
    [StringLength(36)]
    public string SocialInsuranceCode { get; set; } = "";
    public DateTime? SocialInsuranceCreated { get; set; }
    #endregion

    [StringLength(36)]
    public string Username { get; set; } = "";
    public byte[]? PasswordHash { get; set; }
    public byte[]? PasswordSalt { get; set; }
    public int? Timekeeper { get; set; } = 0;
    [StringLength(255)]
    public string Avatar { get; set; } = "";
    [StringLength(255)]
    public string UserRoleIds { get; set; } = "";
    public bool Status { get; set; } = false;
    public DateTime? LastLogin { get; set; }
    public bool RequestPassword { get; set; } = false;
    public bool Quit { get; set; } = false;
    public int YearCurrent { get; set; } = 0;

    #region thu viec
    public DateTime? ProbationFromAt { get; set; }
    public DateTime? ProbationToAt { get; set; }
    public double? SalaryPercentage { get; set; }
    #endregion
    [StringLength(255)]
    public string? SeriPhone { get; set; }

    public int? TimekeeperId { get; set; }
    public int? NumberOfMeals { get; set; }

    public virtual ICollection<UserToContractor>? UserToContracts { get; set; }
}