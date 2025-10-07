namespace ManageEmployee.Entities.GoodsEntities
{
    public class GoodCustomer
    {
        public int Id { get; set; }
        public int GoodId { get; set; }
        public int CustomerId { get; set; }
        public string Type { get; set; }
        public string Note { get; set; }
    }
}
