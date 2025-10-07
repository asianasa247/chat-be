namespace ManageEmployee.Entities.LedgerEntities;

public class LedgerErrorImport
{
    public int Id { get; set; }
    public long LedgerId { get; set; }
    public string? ErrorCodes { get; set; }
    public int IsInternal { get; set; }// 3 noi bo; 1/2 hach toan
    public DateTime CreateAt { get; set; } = DateTime.Now;
}
