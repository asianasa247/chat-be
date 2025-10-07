using ManageEmployee.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.LotteryEntities
{
    public class HistorySpin : BaseEntity
    {
        [Key]
        public int HistoryId { get; set; }
        public int IdSettingsSpin { get; set; }
        public int PrizeId { get; set; }// remove
        public DateTime WinTime { get; set; }
        public DateTime ReceivedDay { get; set; }
        public string? Image { get; set; }
    }

    public class HistorySpinDetail
    {
        public int Id { get; set; }
        public int HistorySpinId { get; set; }
        public int SettingsSpinId { get; set; }
        public int PrizeId { get; set; }
        public int CustomerId { get; set; }
        public int GoodId { get; set; }
        public DateTime WinTime { get; set; }
        public DateTime ReceivedDay { get; set; }
        public string Image { get; set; }
    }
}