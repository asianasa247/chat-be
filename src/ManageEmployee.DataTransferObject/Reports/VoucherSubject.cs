namespace ManageEmployee.DataTransferObject.Reports;

public class VoucherSubject
{
    public string? Title { get; set; }
    public List<VoucherAccount> Debit { get; set; } = new List<VoucherAccount>();// Ghi nợ
    public List<VoucherAccount> Credit { get; set; } = new List<VoucherAccount>(); // Ghi có
    public double TotalDebitAmount
    {
        get
        {
            return Debit.Sum(x => x.Amount);
        }
    }

    public double TotalCreditAmount
    {
        get
        {
            return Credit.Sum(x => x.Amount);
        }
    }

}
