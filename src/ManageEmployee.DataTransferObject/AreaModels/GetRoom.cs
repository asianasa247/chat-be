using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.DataTransferObject.AreaModels
{
    public class GetRoom
    {
        public int RoomId { get; set; }
        public string Code { get; set; }    
        public string Name { get; set; }
        public AreaInfo AreaId { get; set; }
        public FloorInfo FloorId { get; set; }
        public StatusHotelInfo StatusHotelId { get; set; }
        public PriceDayInfo PriceDayId { get; set; }
        public PriceHourInfo PriceHourId { get; set; }
        public GoodInfomation GoodId { get; set; }
    }

    public struct AreaInfo
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public struct FloorInfo
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public struct StatusHotelInfo
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public struct PriceDayInfo
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
    public struct PriceHourInfo
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
    public struct  GoodInfomation
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
