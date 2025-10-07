// File: ManageEmployee.Services/Interfaces/AdditionWebServices/IAdditionWebGoodsService.cs
using ManageEmployee.DataTransferObject.AdditionWebs;
using ManageEmployee.DataTransferObject.SearchModels;

namespace ManageEmployee.Services.Interfaces.AdditionWebServices
{
    public interface IAdditionWebGoodsService
    {
        Task<AdditionWebGoodsPagingResult> GetAllGoodsByWebId(int webId, SearchViewModel param, int year);

        Task<int> SaveGoodsToAdditionWebAsync(int webId, AdditionWebGoodsRequestModel request);

        Task<AdditionWebGoodsPagingResult> GetAllGoodsSelectedByWebId(int webId, SearchViewModel param, int year);
        Task RemoveGoods(int webId, int goodsId);
        Task RemoveGoodsSelected(int webId, List<int> goodsIds);

        // Mới: Lấy TẤT CẢ sản phẩm đã chọn của TẤT CẢ website (không truyền id)
        Task<List<AdditionWebSelectedGroupResult>> GetAllGoodsSelectedAsync(SearchViewModel param, int year);
    }
}
