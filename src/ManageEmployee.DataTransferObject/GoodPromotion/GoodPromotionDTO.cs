using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.DataTransferObject.GoodPromotion
{
    public class GoodPromotionDTO
    {
       
        public int IdSettingsSpin { get; set; }
        public int PrizeId { get; set; }
        public int GoodId { get; set; }
    }
}
