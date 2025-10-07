using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.ConfigurationEntities;

public class Configuration
{
    [Key]
    [MaxLength(100)]
    public string? Key { get; set; }
    [MaxLength(1000)]
    public string? Data { get; set; }
}