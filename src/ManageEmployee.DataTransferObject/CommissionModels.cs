using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.DataTransferObject
{
    public class CommissionModels
    {
        public string Title { get; set; }
        public string Code { get; set; }
        public bool isAmount { get; set; }
        public bool isPercent { get; set; }
    }
}
