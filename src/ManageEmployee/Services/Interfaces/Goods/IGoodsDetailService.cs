using ManageEmployee.DataTransferObject.GoodsModels;

namespace ManageEmployee.Services.Interfaces.Goods;

public interface IGoodsDetailService
{
    Task<IEnumerable<GoodsDetailModel>> GetAllByGood(int goodID);

    Task<List<GoodsDetailModel>> CreateList(List<GoodsDetailModel> param);

    Task Delete(int id);

    Task Update(GoodsDetailModel param);
}
