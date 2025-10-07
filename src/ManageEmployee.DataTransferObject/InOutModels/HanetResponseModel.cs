namespace ManageEmployee.DataTransferObject.InOutModels
{

    public class HanetResponseModel<T> { 
        public string returnCode { get; set; }
        public string returnMessage { get; set; }
        public List<T> data { get; set; }
    }
}
