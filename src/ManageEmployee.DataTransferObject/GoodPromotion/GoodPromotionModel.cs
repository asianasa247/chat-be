using ManageEmployee.DataTransferObject.HistorySpinModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.DataTransferObject.GoodPromotion
{
    public class GoodPromotionModel
    {
        public int GoodPromotionId { get; set; }
        public SettingSpinInfomation SettingsSpinInfo { get; set; }
        public PrizeInfo Prize { get; set; }
        public GoodInfo Good { get; set; }
        public struct SettingSpinInfomation
        {
            public int Id { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
        }
        public struct PrizeInfomation
        {
            public int Id { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
        }
        public struct GoodInfomation
        {
            public int Id { get; set; }
            public string Account { get; set; }


            public string AccountName { get; set; }


            public string Warehouse { get; set; }


            public string WarehouseName { get; set; }


            public string Detail1 { get; set; }


            public string DetailName1 { get; set; }


            public string Detail2 { get; set; }


            public string DetailName2 { get; set; }
        }
    }
}
