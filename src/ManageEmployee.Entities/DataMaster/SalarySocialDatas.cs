using ManageEmployee.Entities.SalaryEntities;

namespace ManageEmployee.Entities.DataMaster;

public static class SalarySocialDatas
{
    public static List<SalarySocial> datas = new()
    {
        new SalarySocial
            {
                Id = 1,
                Code = "KPCD",
                Name = "Kinh phí công đoàn",
                AccountDebit = "",
                DetailDebit1 = "",
                DetailDebit2 = "",
                AccountCredit = "",
                DetailCredit1 = "",
                DetailCredit2 = "",
                Order = 1,
                ValueCompany = 2,
                ValueUser = 0
            },
        new SalarySocial
        {
                Id = 2,
                Code = "BHXH",
                Name = "Bảo hiểm xã hội",
                AccountDebit = "",
                DetailDebit1 = "",
                DetailDebit2 = "",
                AccountCredit = "",
                DetailCredit1 = "",
                DetailCredit2 = "",
                Order = 1,
                ValueCompany = 17,
                ValueUser = 8
            },
        new SalarySocial
        {
                Id = 3,
                Code = "BHYT",
                Name = "Bảo hiểm y tế",
                AccountDebit = "",
                DetailDebit1 = "",
                DetailDebit2 = "",
                AccountCredit = "",
                DetailCredit1 = "",
                DetailCredit2 = "",
                Order = 1,
                ValueCompany = 3,
                ValueUser = 1.5
            },
        new SalarySocial
        {
                Id = 4,
                Code = "BHTN",
                Name = "Bảo hiểm thất nghiệp",
                AccountDebit = "",
                DetailDebit1 = "",
                DetailDebit2 = "",
                AccountCredit = "",
                DetailCredit1 = "",
                DetailCredit2 = "",
                Order = 1,
                ValueCompany = 1,
                ValueUser = 1
            },
        new SalarySocial
        {
                Id = 5,
                Code = "LUONGQUANLY",
                Name = "Lương quản lý",
                AccountDebit = "",
                DetailDebit1 = "",
                DetailDebit2 = "",
                AccountCredit = "",
                DetailCredit1 = "",
                DetailCredit2 = "",
                Order = 0,
                ValueCompany = 0,
                ValueUser = 0
            },
        new SalarySocial
        {
                Id = 6,
                Code = "LUONGNHANVIEN",
                Name = "Lương nhân viên",
                AccountDebit = "",
                DetailDebit1 = "",
                DetailDebit2 = "",
                AccountCredit = "",
                DetailCredit1 = "",
                DetailCredit2 = "",
                Order = 0,
                ValueCompany = 0,
                ValueUser = 0
            },
    };
}
