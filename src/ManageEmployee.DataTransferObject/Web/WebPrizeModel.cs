using ManageEmployee.DataTransferObject.PrizeModels;

namespace ManageEmployee.DataTransferObject.Web
{
    public class WebPrizeModel
    {
        public int PrizeId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public int OrdinalSpin { get; set; }
        public string Note { get; set; }
        public List<PrizeGoodModel> Goods { get; set; }
    }
}
