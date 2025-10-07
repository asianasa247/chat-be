using ManageEmployee.Entities.LedgerEntities;

namespace ManageEmployee.Helpers;

public static class LedgerHelper
{
    public static string GetOriginalVoucher(int maxOriginalVoucher)
    {
        if (maxOriginalVoucher < 10)
            return $"00{maxOriginalVoucher}";
        else if (maxOriginalVoucher < 100)
            return $"0{maxOriginalVoucher}";
        else
            return maxOriginalVoucher.ToString();
    }



    public static Ledger LedgerInit()
    {
        return new Ledger()
        {
            BookDate = DateTime.Today,
            InvoiceDate = DateTime.Today,
            OrginalBookDate = DateTime.Today,
            ReferenceBookDate = DateTime.Today,
            Month = DateTime.Today.Month,
            OrginalCode = "",
            OrginalFullName = "",
            OrginalDescriptionEN = "",
            ReferenceVoucherNumber = "",
            ReferenceFullName = "",
            ReferenceAddress = "",
            InvoiceProductItem = "",
            ProjectCode = "",
            IsVoucher = false,
            IsInternal = 1,
            CreditCode = "5111",
            CreateAt = DateTime.Now,
        };

    }
}
