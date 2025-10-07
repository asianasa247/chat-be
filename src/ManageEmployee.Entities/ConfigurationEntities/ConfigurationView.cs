using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.ConfigurationEntities;

public class ConfigurationView
{
    public int Id { get; set; }

    [StringLength(36)]
    public string? ViewName { get; set; }

    [StringLength(36)]
    public string? FieldName { get; set; }

    [StringLength(500)]
    public string? Value { get; set; }
}