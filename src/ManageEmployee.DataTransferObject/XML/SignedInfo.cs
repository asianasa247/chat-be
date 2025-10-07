namespace ManageEmployee.DataTransferObject.XML;

public class SignedInfo
{
    public string? CanonicalizationMethod { get; set; }
    public string? SignatureMethod { get; set; }
    public Reference? Reference { get; set; }
    public string? DigestMethod { get; set; }
    public string? DigestValue { get; set; }
}
