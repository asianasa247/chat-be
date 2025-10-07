using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.Web;
using ManageEmployee.Entities.CategoryEntities;
using ManageEmployee.Entities.GoodsEntities;
using ManageEmployee.Services.Web;
using GoodsEntity = ManageEmployee.Entities.GoodsEntities.Goods;

namespace ManageEmployee.Services.Interfaces.Webs;

public interface IWebProductService
{
    Task<List<GoodsEntity>> GetTopProductSell();

    Task<CommonWebResponse> GetProduct(ProductSearchModel search);

    Task<ProductsDetailResponse> GetByIdAsync(int id);

    Task<List<WebProductByCategory>> GetProductCategory();

    Task<List<GoodsEntity>> GetProductsByMenuTypeAsync(string menuType);
    Task<ProductPagging> GetProductsPagging(int pageNum = 0, int pageSize = 10, string q = "");
    Task<Category> GetCategoryByCodeAsync(string code);
    Task<PagingResult<GoodsEntity>> GetProductsByMenuTypeAsync(string menuType, PagingRequestModel param, bool isService);
    Task<List<ProductsByMenuTypeResponse>> GetProductsByMenuTypeAsyncV2(string menuType);
    Task<List<GoodsWebs>> GetProductsByMenuTypeAndAdditionAsync(string name);
    Task<List<GoodsWebs>> GetProductsByMenuWebAsync(string name);
}
