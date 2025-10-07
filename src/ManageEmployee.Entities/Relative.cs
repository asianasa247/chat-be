using System.ComponentModel.DataAnnotations;
using ManageEmployee.Entities.BaseEntities;
using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.Entities;

public class Relative : BaseEntity
{
    public int Id { get; set; }
    [StringLength(255)]
    public string? Code { get; set; }
    [StringLength(255)]
    public string? FullName { get; set; }
    [StringLength(255)]
    public string? Phone { get; set; }
    public DateTime? BirthDay { get; set; }
    [StringLength(255)]
    public string? Email { get; set; }
    [StringLength(255)]
    public string? Avatar { get; set; }
    [StringLength(255)]
    public string? Address { get; set; }
    public int CompanyId { get; set; }
    public GenderEnum Gender { get; set; }
    public bool Status { get; set; }
    public DateTime? LastLogin { get; set; }
    public double Total { get; set; }
    public bool Quit { get; set; }
    public int? ProvinceId { get; set; }
    public int? DistrictId { get; set; }
    public int? WardId { get; set; }

    #region CMND
    [StringLength(255)]
    public string? Identify { get; set; }
    [StringLength(255)]
    public string? NativePlace { get; set; }
    [StringLength(255)]
    public string? PlaceOfPermanent { get; set; }
    public DateTime? IdentifyCreatedDate { get; set; }
    public DateTime? IdentifyExpiredDate { get; set; }
    public string? Religion { get; set; } // Tôn giáo
    [StringLength(255)]
    public string? EthnicGroup { get; set; } // Dân tộc
    public int UnionMember { get; set; } = 0;// Đoàn viên
    [StringLength(255)]
    public string? Nation { get; set; } // Quốc gia
    [StringLength(255)]
    public string? IdentifyCreatedPlace { get; set; }
    public int? NativeProvinceId { get; set; }
    public int? NativeDistrictId { get; set; }
    public int? NativeWardId { get; set; }
    #endregion


    #region Trình độ
    [StringLength(255)]
    public string? Literacy { get; set; } // trình dộ học vấn
    [StringLength(255)]
    public string? Degree { get; set; }// bằng cấp
    public int? MajorId { get; set; }// chuyên môn
    [StringLength(255)]
    public string? CertificateOther { get; set; } // chứng chỉ
    #endregion

}