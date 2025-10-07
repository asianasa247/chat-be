using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.LotteryEntities
{
    public class PrizeGood
    {
        [Key]
        public int Id { get; set; }

        public int IdSettingsSpin { get; set; }
        public int PrizeId { get; set; }
        public int GoodId { get; set; }
    }
}