using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManageEmployee.Entities.ConfigurationEntities;

[Table("ConfigAriseDocumentBehaviour")]
public class ConfigAriseDocumentBehaviour
{
    [Key]
    public int Id { get; set; }
    public int AriseBehaviourId { get; set; }
    public int DocumentId { get; set; }
    public bool NokeepDataChartOfAccount { get; set; }
    public bool NokeepDataBill { get; set; }
    public bool NokeepDataTax { get; set; }
    public bool FocusLedger { get; set; }
    public string? FocusFunctions { get; set; }

    [ForeignKey("AriseBehaviourId")]
    public ConfigAriseBehaviour? ConfigAriseBehaviour { get; set; }
}
