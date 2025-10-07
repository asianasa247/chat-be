using ManageEmployee.Entities.LedgerEntities;

namespace ManageEmployee.DataTransferObject.LedgerWarehouseModels;

public class LedgerWarehouseDetail
{
    public int Id { get; set; }
    public int? CustomerId { get; set; }
    public int? Month { get; set; }
    public int? IsInternal { get; set; }
    public string? Type { get; set; }
    public List<Ledger>? ledgers { get; set; }
}