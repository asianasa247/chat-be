using ManageEmployee.Entities.BaseEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.Entities.AreaEntities
{
    public class PriceDay : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }    
        public string? Code { get; set; }
        public decimal? Price { get; set; }
        public DateTime FromHour { get; set; }
        public DateTime ToHour { get; set; }
    }
}
