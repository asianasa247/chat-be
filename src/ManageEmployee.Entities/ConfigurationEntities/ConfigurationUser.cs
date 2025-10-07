using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.ConfigurationEntities;

public class ConfigurationUser
{
    public int Id { get; set; }
    [MaxLength(100)]
    public string? Key { get; set; }

    [MaxLength(1000)]
    public string? Data { get; set; }
    public int UserId { get; set; }
}