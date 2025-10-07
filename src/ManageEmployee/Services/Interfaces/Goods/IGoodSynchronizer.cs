namespace ManageEmployee.Services.Interfaces.Goods
{
    public interface IGoodSynchronizer
    {
        Task<bool> CheckGoodNew(int year);
        Task SetGoodFromAccountAsync(List<GoodChartOfAccountUpdateModel> listAccount);
        Task SyncAccountGood(int year);
    }
}
