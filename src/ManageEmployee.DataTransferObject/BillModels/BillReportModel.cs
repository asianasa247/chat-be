namespace ManageEmployee.DataTransferObject.BillModels;

public class BillReporterModel
{
    public double AverageUnitPrice { get; set; }//don gia binh quan
    public double TonsCollected { get; set; }//so tan thu ve
    public double TotalQuantity { get; set; }//so luong
    public double AmountBonus { get; set; }// thuong
    public double ReceiveBonus { get; set; }//nhan thuong %
    public double ToTalAmount { get; set; }// nhan thuong
    public BillReporterDetailModel? TotalItem { get; set; }
    public List<BillReporterDetailModel>? Items { get; set; }

}
