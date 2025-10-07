using ManageEmployee.Entities.UserEntites;

namespace ManageEmployee.Entities.DataMaster;

public static class AccountPayData
{
    public static List<AccountPay> datas = new()
    {
        new AccountPay
            {
                Id = 1,
                Code = "TM",
                Name = "Tiền mặt",
                Account = "1111",
                AccountName = "Tiền Việt Nam",
                Detail1 = "TM",
                DetailName1 = "Tiền mặt tại quỹ",
                Detail2 = "",
                DetailName2 = "",
            },
        new AccountPay
            {
                Id = 2,
                Code = "VAT",
                Name = "Thuế VAT",
                Account = "33311",
                AccountName = "Thuế GTGT đầu ra",
                Detail1 = "",
                DetailName1 = "",
                Detail2 = "",
                DetailName2 = "",
            },
        new AccountPay
            {
                Id = 3,
                Code = "NH",
                Name = "Ngân hàng",
                Account = "1121",
                AccountName = "Tiền Việt Nam",
                Detail1 = "BIDV",
                DetailName1 = "Ngân hàng BIDV",
                Detail2 = "",
                DetailName2 = "",
            },
        new AccountPay
            {
                Id = 4,
                Code = "DV",
                Name = "Thuế đầu vào",
                Account = "1331",
                AccountName = "Thuế GTGT được khấu trừ của hàng hóa, dịch vụ",
                Detail1 = "",
                DetailName1 = "",
                Detail2 = "",
                DetailName2 = "",
            },
    };
}
