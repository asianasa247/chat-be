
using ManageEmployee.DataTransferObject.GoodsModels;
using ManageEmployee.Entities;

namespace ManageEmployee.Services.Interfaces.Goods
{
    public interface IGoodSetter
    {
        Task<bool> CheckExistGoods(Entities.GoodsEntities.Goods requests);
        Task<string> Create(GoodsUpdateModel param, int year);
        Task Delete(int id);
        Task<string> Update(GoodsUpdateModel param, int year);
        Task UpdateGoodIsService(List<int> goodIds);
        Task<string> UpdateGoodsWebsite(Entities.GoodsEntities.Goods requests);
        Task UpdateMenuTypeForGood(UpdateMenuTypeForGoodModel request);
        Task UpdateStatusGoods(List<int> goodIds, int status);
    }
}
