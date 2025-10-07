using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.DataTransferObject.AreaModels
{
    public class GetFloorModel
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public int AreaId { get; set; }
        public string AreaName { get; set; }
    }
}
