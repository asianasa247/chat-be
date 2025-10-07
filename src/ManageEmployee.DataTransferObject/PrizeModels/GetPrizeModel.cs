using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.DataTransferObject.PrizeModels
{
    public class GetPrizeModel
    {
        public int PrizeId { get; set; }
        public string Code { get; set; }
        public string? Name { get; set; }
        public int IdSettingsSpin { get; set; }
        public string NameSettingSpin { get; set; }
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public int OrdinalSpin { get; set; }
        public string? Note { get; set; }
        public List<PrizeGoodModel> Goods { get; set; }
    }

    public class PrizeGoodModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Image { get; set; }
    }
}
