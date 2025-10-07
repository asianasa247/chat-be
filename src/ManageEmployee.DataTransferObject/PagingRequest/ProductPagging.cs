using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.DataTransferObject.PagingRequest
{
    public class ProductPagging
    {
        public object Products { get; set; } 
        public int Count { get; set; }
    }
}
