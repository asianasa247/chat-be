using ManageEmployee.DataTransferObject.Email;
using ManageEmployee.DataTransferObject.Zalo;
using ManageEmployee.Entities.Email;
using ManageEmployee.Entities.ZaloEntities;
using System.Linq.Expressions;

namespace ManageEmployee.Services.Interfaces.Zalo
{
    public interface IZaloAppConfigService
    {
        Task<List<ZaloAppConfig>> GetAll();
        Task<IEnumerable<ZaloAppConfig>> GetMany(Expression<Func<ZaloAppConfig, bool>> filter = null);
        Task<ZaloAppConfig> GetById(int id);
        Task<ZaloAppConfig> Create(ZaloAppConfigModel model);
        Task<ZaloAppConfig> Update(ZaloAppConfigModel model);
        Task<bool> Delete(int id);
    }
}
