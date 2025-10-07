using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ManageEmployee.DataTransferObject.AreaModels
{
    public class PriceDayDTO
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
        public decimal? Price { get; set; }
        public DateTime? FromHour { get; set; }
        public DateTime? ToHour { get; set; }

       
    }
}
