namespace ManageEmployee.DataTransferObject.Reports;

public class LedgerReportCalculatorIO
{
    public double OpeningStockQuantity { get; set; }
    public bool IsInput { get; set; } = false;
    public double StockUnitPrice { get; set; }
    public double OpeningDebit { get; set; }
    public double OpeningCredit { get; set; }
    public double OpeningAmountLeft { get; set; }

    /// <summary>
    /// 1; Dư đầu kỳ
    /// 2: Cộng phát sinh
    /// 3: Lũy kế phát sinh
    /// 4: Dư cuối tháng
    /// </summary>
    public int RowType { get; set; }
    public long MonthYear { get; set; }
    //
    public double ExchangeRate { get; set; }
    public double OriginalCurrency { get; set; }

    #region noi bo
    public double OpeningDebitNB { get; set; }
    public double OpeningCreditNB { get; set; }
    public double OpeningAmountLeftNB { get; set; }
    public double OriginalCurrencyNB { get; set; }
    public double ExchangeRateNB { get; set; }
    public double StockUnitPriceNB { get; set; }
    public double OpeningStockQuantityNB { get; set; }
    #endregion
}
