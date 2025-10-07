using ManageEmployee.Entities.BaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.Entities
{
    public class Commission: BaseEntity
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Code { get; set; }
        public bool isAmount { get; set; }
        public bool isPercent { get; set; }=false;
    }
}
