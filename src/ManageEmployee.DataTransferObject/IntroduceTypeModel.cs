using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.DataTransferObject
{
    public class IntroduceTypeModel
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Note { get; set; }
        public int? OrdinalNumber { get; set; }
        public int? Types { get; set; }
    }
}
