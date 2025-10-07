
namespace ManageEmployee.DataTransferObject.GoodsModels
{
    public class GoodCustomerModel
    {
        public int Id { get; set; }
        public int GoodId { get; set; }
        public int CustomerId { get; set; }
        public string Note { get; set; }
    }
}
