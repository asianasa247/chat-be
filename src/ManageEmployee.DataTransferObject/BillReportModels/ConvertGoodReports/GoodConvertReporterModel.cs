namespace ManageEmployee.DataTransferObject.BillReportModels.ConvertGoodReports
{
    public class GoodConvertReporterModel
    {
        public string Warehouse { get; set; }
        public string Account { get; set; }
        public string Detail1 { get; set; }
        public string Detail2 { get; set; }
        public double OpenQuantity { get; set; }
        public double InputQuantity { get; set; }
        public double OutputQuantity { get; set; }
        public double CloseQuantity { get; set; }
        public string GoodName { get; set; }
        public string GoodCode { get; set; }
    }
    public class GoodNormalReporterModel : GoodConvertReporterModel
    {

        public double? OppositeOpenQuantity { get; set; }
        public double? OppositeInputQuantity { get; set; }
        public double? OppositeOutputQuantity { get; set; }
        public double? OppositeCloseQuantity { get; set; }
        public string OppositeGoodName { get; set; }
        public string OppositeGoodCode { get; set; }
    }

    public class GoodConvertBeforeReporterModel : GoodConvertReporterModel
    {
        public string GoodQuantity { get; set; }
        public string OppositeGoodQuantity { get; set; }
    }
}