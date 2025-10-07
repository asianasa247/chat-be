using ManageEmployee.Entities.Enumerations;
using Microsoft.AspNetCore.Http;

namespace ManageEmployee.DataTransferObject;

public class RelativeViewModel
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? FullName { get; set; }
    public string? Phone { get; set; }
    public DateTime? BirthDay { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? Language { get; set; }
    public int Timekeeper { get; set; } = 0;
    public int CompanyId { get; set; }
    public GenderEnum Gender { get; set; }
    public bool Status { get; set; } = false;

    public string? NoteUser { get; set; }

    #region CMND
    public string? Identify { get; set; }
    public string? NativePlace { get; set; }
    public string? PlaceOfPermanent { get; set; }
    public DateTime? IdentifyCreatedDate { get; set; }
    public DateTime? IdentifyExpiredDate { get; set; }
    public string? Religion { get; set; } // Tôn giáo
    public string? EthnicGroup { get; set; } // Dân tộc
    public int UnionMember { get; set; } = 0; // Đoàn viên
    public string? Nation { get; set; } // Quốc gia
    public string? IdentifyCreatedPlace { get; set; }

    #endregion


    #region Trình độ
    public string? Literacy { get; set; }
    public string? LiteracyDetail { get; set; }
    public string? Specialize { get; set; }
    public string? Certificate { get; set; } // Tôn giáo
    #endregion

    #region Lương
    public double Total { get; set; } = 0;
    #endregion
    public IFormFile? AvataFile { get; set; }
    public bool Quit { get; set; } = false;

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
