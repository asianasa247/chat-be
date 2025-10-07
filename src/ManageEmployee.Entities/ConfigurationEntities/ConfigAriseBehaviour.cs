using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManageEmployee.Entities.ConfigurationEntities;

[Table("ConfigAriseBehaviour")]
public class ConfigAriseBehaviour
{
    [Key]
    public int Id { get; set; }
    [StringLength(100)]
    public string? CodeData { get; set; }
    [StringLength(200)]
    public string? Name { get; set; }
    [StringLength(200)]
    public string? Code { get; set; }
    public int Index { get; set; }
    public int Order { get; set; }
    public IEnumerable<ConfigAriseDocumentBehaviour>? ConfigAriseDocumentBehaviours { get; set; }
}
