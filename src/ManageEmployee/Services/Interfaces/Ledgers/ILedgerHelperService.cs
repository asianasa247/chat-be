using ManageEmployee.Entities.Enumerations;

public interface ILedgerHelperService
{
    void SetAutoIncrement(DateTime dtEntryDate, AssetsType eType, int isInternal, int year, int? iSeed = 1);
    Task<int> GetOriginalVoucher(bool isNotInternal, string type, int year);

    string OrginalVoucherNumber { get; }
    string VoucherNumber { get; }
}
