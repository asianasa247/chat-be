using ManageEmployee.Entities.BaseEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.Entities.LotteryEntities
{
    public class Prize : BaseEntity
    {
        [Key]
        public int PrizeId { get; set; }
        public string Code { get; set; }
        public string? Name { get; set; }
        public int IdSettingsSpin {  get; set; }   
        public string? Description { get; set; } 
        public int Quantity { get; set; }
        public int OrdinalSpin { get; set; }
        public string? Note { get; set; }

    }
}
