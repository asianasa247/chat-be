namespace ManageEmployee.DataTransferObject.AriseModels;

public class ConfigAriseDocumentBehaviourInputDto
{
    public string? Key { get; set; }
    public int AriseBehaviourId { get; set; }
    public int DocumentId { get; set; }
    public bool Value { get; set; }
    public string? Function { get; set; }
}
