namespace ManageEmployee.DataTransferObject.XML;

public class Signature
{
    public SignedInfo? SignedInfo { get; set; }
    public string? SignatureValue { get; set; }
    public KeyInfo? KeyInfo { get; set; }
}
