using ManageEmployee.Entities.BillEntities;
using ManageEmployee.Entities.CustomerEntities;

namespace ManageEmployee.DataTransferObject.BillModels;

public class BillForCustomerInvoice
{
    public Bill? Bill { get; set; }
    public IEnumerable<BillDetail>? BillDetails { get; set; }
    public CustomerTaxInformation? Customer { get; set; }
}
