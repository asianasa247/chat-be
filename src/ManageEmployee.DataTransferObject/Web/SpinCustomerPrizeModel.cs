
namespace ManageEmployee.DataTransferObject.Web
{
    public class SpinCustomerPrizeModel
    {
        public int SettingsSpinId { get; set; }
        public string SettingsSpinCode { get; set; }
        public string SettingsSpinName { get; set; }
        public List<SpinCustomerPrizeDetailModel> Details { get; set; }
    }

    public class SpinCustomerPrizeDetailModel
    {
        public int PrizeId { get; set; }
        public string PrizeCode { get; set; }
        public string PrizeName { get; set; }
        public int CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public int GoodId { get; set; }
        public string GoodName { get; set; }

    }
}
