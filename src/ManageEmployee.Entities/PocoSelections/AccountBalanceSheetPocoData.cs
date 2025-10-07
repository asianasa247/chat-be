namespace ManageEmployee.Entities.PocoSelections;

public class AccountBalanceSheetPocoData : AccountBalanceSheetPoco
{
    public double? CumulativeDebitAmount { get; set; }// luy ke
    public double? CumulativeCreditAmount { get; set; }
    public double? EndCreditAmount { get; set; }
    public double? EndDebitAmount { get; set; }
}
