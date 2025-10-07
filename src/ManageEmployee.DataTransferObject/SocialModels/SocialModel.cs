using ManageEmployee.Entities.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.DataTransferObject;

public class SocialModel
{
    [Required]
    public string Token { get; set; }

    [Required]
    public string Provider { get; set; }
}
