using ManageEmployee.Entities.GoodsEntities;

namespace ManageEmployee.Helpers;

public static class GoodNameGetter
{
    public static string GetNameFromGood( Goods good)
    {
        if (!string.IsNullOrEmpty(good.DetailName2))
        {
            return good.DetailName2;
        }

        if (!string.IsNullOrEmpty(good.DetailName1))
        {
            return good.DetailName1;
        }

        return good.AccountName;
    }

    public static string GetCodeFromGood(Goods good)
    {
        if (!string.IsNullOrEmpty(good.Detail2))
        {
            return good.Detail2;
        }

        if (!string.IsNullOrEmpty(good.Detail1))
        {
            return good.Detail1;
        }

        return good.Account;
    }

    public static string GetNameFromGoodDetail(GoodDetail good)
    {
        if (!string.IsNullOrEmpty(good.DetailName2))
        {
            return good.DetailName2;
        }

        if (!string.IsNullOrEmpty(good.DetailName1))
        {
            return good.DetailName1;
        }

        return good.AccountName;
    }

    public static string GetCodeFromGoodDetail(GoodDetail good)
    {
        if (!string.IsNullOrEmpty(good.Detail2))
        {
            return good.Detail2;
        }

        if (!string.IsNullOrEmpty(good.Detail1))
        {
            return good.Detail1;
        }

        return good.Account;
    }
}
