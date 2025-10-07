using ManageEmployee.DataTransferObject.Email;
using ManageEmployee.DataTransferObject.Zalo;
using ManageEmployee.Entities.Email;
using ManageEmployee.Entities.ZaloEntities;
using System.Linq.Expressions;

namespace ManageEmployee.Services.Interfaces.Zalo
{
    public interface IZaloUserService
    {
        Task<List<ZaloUser>> GetAll();
        Task<IEnumerable<ZaloUser>> GetMany(Expression<Func<ZaloUser, bool>> filter = null);
        Task<ZaloUser> GetById(int id);
        Task<ZaloUser> Create(ZaloUserModel model);
        Task<ZaloUser> Update(ZaloUserModel model);
        Task<bool> Delete(int id);
    }
}
