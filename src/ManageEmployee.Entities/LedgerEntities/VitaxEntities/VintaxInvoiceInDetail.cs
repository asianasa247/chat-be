using ManageEmployee.Entities.BaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.Entities.LedgerEntities.VitaxEntities
{
    public class VintaxInvoiceInDetail : BaseEntity
    {
        public int Id { get; set; }

        public string InvoiceId { get; set; }
        public string GoodName { get; set; }
        public double? Quantity { get; set; }
        public double? UnitPrice { get; set; }
        public double? Amount { get; set; }
        public string StockUnit { get; set; }
    }
}
