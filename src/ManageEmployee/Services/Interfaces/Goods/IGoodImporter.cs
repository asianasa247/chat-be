using ManageEmployee.DataTransferObject.GoodsModels;

namespace ManageEmployee.Services.Interfaces.Goods
{
    public interface IGoodImporter
    {
        Task ImportFromExcel(List<GoodsExportlModel> lstGoods, bool isManager);
    }
}
