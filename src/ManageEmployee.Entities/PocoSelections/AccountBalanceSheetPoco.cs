namespace ManageEmployee.Entities.PocoSelections;

public class AccountBalanceSheetPoco
{
    public long Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? ParentRef { get; set; }
    public string? WareHouseCode { get; set; }
    public string? WareHouseName { get; set; }
    public int AccountType { get; set; }
    public double? OpenDebitAmount { get; set; }
    public double? OpenCreditAmount { get; set; }
    public double? ArisingDebitAmount { get; set; }
    public double? ArisingCreditAmount { get; set; }
}
