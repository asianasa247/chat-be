namespace ManageEmployee.DataTransferObject.AriseModels;

public class ConfigAriseDocumentBehaviourDto
{
    public int Id { get; set; }
    public int AriseBehaviourId { get; set; }
    public int DocumentId { get; set; }
    public bool NokeepDataChartOfAccount { get; set; }
    public bool NokeepDataBill { get; set; }
    public bool NokeepDataTax { get; set; }
    public bool FocusLedger { get; set; }
    public List<string>? FocusFunctions { get; set; }
    public ConfigAriseBehaviourDto ConfigAriseBehaviour { get; set; } = new ConfigAriseBehaviourDto();
}
