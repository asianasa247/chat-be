namespace ManageEmployee.DataTransferObject.FixedAssetsModels;

public class FixedAssetsModelEdit
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public DateTime? UsedDate { get; set; } //ngày đưa vào sử dụng
    public double? HistoricalCost { get; set; } //nguyên giá
    public double? UsedQuantity { get; set; } //nguyên giá
    public int? TotalMonth { get; set; } //số tháng khấu hao
    public string? Type { get; set; }

    //Tính
    public DateTime PeriodDate { get; set; }
    public int? TotalMonthLeft { get; set; }
    public int? TotalDayDepreciationOfThisPeriod { get; set; } //số ngày sử dụng hiện tại
    public double? DepreciationOfOneDay { get; set; } //giá trị khấu hao 1 ngày
    public double? DepreciationOfThisPeriod { get; set; } //giá trị khấu hao trong kỳ
    public double? CarryingAmount { get; set; } //giá trị còn lại
    public string? DebitCode { get; set; }
    public string? DebitDetailCodeFirst { get; set; }
    public string? DebitDetailCodeSecond { get; set; }
    public string? CreditCode { get; set; }
    public string? CreditDetailCodeFirst { get; set; }
    public string? CreditDetailCodeSecond { get; set; }
    public DateTime? EndOfDepreciation { get; set; }
    public int? DepartmentId { get; set; }
    public int? UserId { get; set; }
    public double? UsedDateUnix { get; set; } //ngày đưa vào sử dụng
    public short Use { get; set; }

    public DateTime? BuyDate { get; set; }
    public double? Quantity { get; set; }
    public double? UnitPrice { get; set; }
    public string? AttachVoucher { get; set; }
    public void Calculate(DateTime dtPeriodDate)
    {
        PeriodDate = dtPeriodDate;
        TotalMonthLeft = TotalMonth - (dtPeriodDate.Year * 12 + dtPeriodDate.Month - (UsedDate.Value.Year * 12 + UsedDate.Value.Month));

        int iDaysInMonth = DateTime.DaysInMonth(dtPeriodDate.Year, dtPeriodDate.Month);
        if (TotalMonthLeft > 0)
        {
            if (TotalMonthLeft == TotalMonth)
            {
                TotalDayDepreciationOfThisPeriod = iDaysInMonth - UsedDate.Value.Day + 1;
                DepreciationOfOneDay = HistoricalCost / (TotalMonth * iDaysInMonth);
                DepreciationOfThisPeriod = DepreciationOfOneDay * TotalDayDepreciationOfThisPeriod;
                CarryingAmount = HistoricalCost - DepreciationOfThisPeriod;
            }
            else
            {
                TotalDayDepreciationOfThisPeriod = iDaysInMonth;
                DepreciationOfOneDay = (HistoricalCost ?? 0) / (TotalMonth == 0 ? 1 : TotalMonth) / iDaysInMonth;
                DepreciationOfThisPeriod = DepreciationOfOneDay * TotalDayDepreciationOfThisPeriod;
                double ReDepreciationOfThisPeriod = DepreciationOfThisPeriod ?? 0;
                int ReTotalMonthLeft = (TotalMonthLeft ?? 0) + 1;
                int RedtPeriodDate = dtPeriodDate.Month == 1 ? 13 : dtPeriodDate.Month;
                for (int i = 0; i < (TotalMonth ?? 0); i++)
                {
                    RedtPeriodDate--;
                    if (RedtPeriodDate < 1)
                        RedtPeriodDate = 12;
                    int ReDaysInMonth = DateTime.DaysInMonth(dtPeriodDate.Year, RedtPeriodDate);

                    if (ReTotalMonthLeft == TotalMonth)
                    {
                        if (UsedDate.Value.Month == RedtPeriodDate)
                            ReDepreciationOfThisPeriod += (HistoricalCost / (TotalMonth * ReDaysInMonth) ?? 0) * (ReDaysInMonth - UsedDate.Value.Day + 1);
                        else
                            ReDepreciationOfThisPeriod += HistoricalCost / TotalMonth ?? 0;

                        CarryingAmount = (HistoricalCost ?? 0) - ReDepreciationOfThisPeriod;
                        break;
                    }
                    else
                    {
                        ReTotalMonthLeft = ReTotalMonthLeft + 1;
                        ReDepreciationOfThisPeriod += ((HistoricalCost ?? 0) / (TotalMonth == 0 ? 1 : TotalMonth) / ReDaysInMonth ?? 0) * ReDaysInMonth;
                    }

                    if ((RedtPeriodDate - 2) % 12 == (UsedDate.Value.Month + (TotalMonth ?? 0) % 12) % 12
                        && Math.Round((HistoricalCost ?? 0) - ReDepreciationOfThisPeriod, 0) <= Math.Round(DepreciationOfThisPeriod ?? 0, 0))
                    {
                        ReDepreciationOfThisPeriod += (HistoricalCost / (TotalMonth * ReDaysInMonth) ?? 0) * (ReDaysInMonth - UsedDate.Value.Day + 1);
                        CarryingAmount = 0;
                        DepreciationOfThisPeriod = (HistoricalCost ?? 0) - ReDepreciationOfThisPeriod;
                        break;
                    }
                }
            }

        }
        else if (TotalMonthLeft == 0)
        {
            DepreciationOfOneDay = HistoricalCost / (TotalMonth * iDaysInMonth);

            // thang dau tien
            var dayInFirstMonth = DateTime.DaysInMonth(dtPeriodDate.Year, UsedDate.Value.Month);
            var dayLeft = dayInFirstMonth - UsedDate.Value.Day + 1;
            var amountOfOneDay = HistoricalCost / (TotalMonth * dayInFirstMonth);
            var amountLeft = amountOfOneDay * dayLeft;

            // thang con lại
            amountLeft += (HistoricalCost / TotalMonth ?? 0) * (TotalMonth - 1);

            DepreciationOfThisPeriod = HistoricalCost - amountLeft;
            CarryingAmount = 0;
        }
        else
        {
            if (dtPeriodDate.Month == 1)
            {
                iDaysInMonth = DateTime.DaysInMonth(dtPeriodDate.Year, 1);
                TotalDayDepreciationOfThisPeriod = iDaysInMonth;
                DepreciationOfOneDay = (HistoricalCost ?? 0) / (TotalMonth == 0 ? 1 : TotalMonth) / iDaysInMonth;

                double ReDepreciationOfThisPeriod = DepreciationOfThisPeriod ?? 0;
                int ReTotalMonthLeft = (TotalMonthLeft ?? 0) + 1;
                int RedtPeriodDate = 13;
                int RedtPeriodYear = dtPeriodDate.Year;
                for (int i = 0; i <= (TotalMonth ?? 0); i++)
                {
                    RedtPeriodDate--;
                    if (RedtPeriodDate == 0)
                    {
                        RedtPeriodDate = 12;
                        RedtPeriodYear--;
                    }
                    int ReDaysInMonth = DateTime.DaysInMonth(RedtPeriodYear, RedtPeriodDate);
                    if (ReTotalMonthLeft == TotalMonth)
                    {
                        ReDepreciationOfThisPeriod += (HistoricalCost / (TotalMonth * ReDaysInMonth) ?? 0) * (ReDaysInMonth - UsedDate.Value.Day + 1);
                        break;
                    }
                    else
                    {
                        ReTotalMonthLeft = ReTotalMonthLeft + 1;
                        ReDepreciationOfThisPeriod += ((HistoricalCost ?? 0) / (TotalMonth == 0 ? 1 : TotalMonth) / ReDaysInMonth ?? 0) * ReDaysInMonth;
                    }
                }

                DepreciationOfThisPeriod = (CarryingAmount ?? 0) - ReDepreciationOfThisPeriod;
                CarryingAmount = 0;
            }
            else
            {
                TotalMonthLeft = 0;
                DepreciationOfOneDay = 0;
                DepreciationOfThisPeriod = 0;
                CarryingAmount = 0;
            }
        }

        DepreciationOfOneDay = Math.Round(DepreciationOfOneDay ?? 0);
        DepreciationOfThisPeriod = Math.Round(DepreciationOfThisPeriod ?? 0);
        CarryingAmount = Math.Round(CarryingAmount ?? 0);
    }
}
