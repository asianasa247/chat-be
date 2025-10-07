using ManageEmployee.DataTransferObject.CustomerModels;

namespace ManageEmployee.DataTransferObject.Web
{
    public class WebSpinWithCustomerModel
    {

        public int SettingId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public DateTime TimeStartSpin { get; set; }
        public int TimeStartPerSpin { get; set; }
        public int TimeStopPerSpin { get; set; }
        public DateTime AwarDay { get; set; }
        public string Note { get; set; }
        public List<CustomerModelView> Customers { get; set; }
    }
}