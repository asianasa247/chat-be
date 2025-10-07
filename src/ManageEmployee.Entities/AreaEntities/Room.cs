using ManageEmployee.Entities.BaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.Entities.AreaEntities
{
    public class Room : BaseEntity
    {
        public int RoomId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int AreaId { get; set; }
        public int FloorId { get; set; }
        public int StatusHotelId { get; set; }
        public int GoodId { get; set; }
        public int PriceDayId { get; set; }
        public int PriceHourId { get; set; }

    }
}
