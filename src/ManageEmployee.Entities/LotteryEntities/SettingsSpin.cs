using ManageEmployee.Entities.BaseEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.Entities.LotteryEntities 
{
    
    public class SettingsSpin : BaseEntity
    {
        [Key]
        public int SettingId { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public int IdCustomerClassification { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public DateTime TimeStartSpin { get; set; }
        public int TimeStartPerSpin {  get; set; }
        public int TimeStopPerSpin { get; set; }
        public DateTime AwarDay { get; set; }
        public string? Note { get; set; }

    }
}
