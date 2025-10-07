using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities;

public class Subsidize
{
    public int Id { get; set; }
    [StringLength(36)]
    public string? Name { get; set; }
    [StringLength(36)]
    public string? Code { get; set; }
    public int SortCode { get; set; }
    public bool IsDeleted { get; set; }
}