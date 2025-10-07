namespace ManageEmployee.DataTransferObject;

public class CertificateModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int CompanyId { get; set; } = 0;
    public bool Status { get; set; } = false;
}