namespace ManageEmployee.DataTransferObject;

public class TillManagerModel
{
    public int Id { get; set; }
    public decimal FromAmount { get; set; }
    public decimal ToAmountAuto { get; set; }
    public bool IsDifferentMoney { get; set; }
    public decimal? AmountDifferent { get; set; }
    public bool IsFinish { get; set; }
}
