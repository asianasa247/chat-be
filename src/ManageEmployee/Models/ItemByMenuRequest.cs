using GoodsEntity = ManageEmployee.Entities.GoodsEntities.Goods;
namespace ManageEmployee.Models
{
    public class ItemByMenuRequest
    {
        public List<ItemByMenuCategory> MenuTypes { get; set; }=new List<ItemByMenuCategory>();
    }
    public class ItemByMenuCategory
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string? NameEnglish { get; set; }
        public string? NameKorea { get; set; }
        public string? Name { get; set; }
        public string? Icon { get; set; }
    }
    public class ItemNews
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public string TitleEnglish { get; set; }
        public string TitleKorean { get; set; }
        public List<DataTransferObject.FileModels.FileDetailModel> Images { get; set; } = new List<DataTransferObject.FileModels.FileDetailModel>();
        public DateTime? CreateAt { get; set; }
        public string? Author { get; set; }
        public string? PublishDate { get; set; }
    }
    public class ItemByMenuResponse
    {
        public ItemByMenuCategory Category { get; set;}
        public bool IsProduct { get; set; } = true;
        public List<GoodsEntity> Products { get; } = new List<GoodsEntity>();
        public List<ItemNews> News { get; } = new List<ItemNews>();
    }
    public class ProductPagging
    {
        public List<GoodsEntity> Products { get; set; } = new List<GoodsEntity>();
        public int Count { get; set; }
    }
}
