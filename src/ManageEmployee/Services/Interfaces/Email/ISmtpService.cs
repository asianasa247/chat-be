using ManageEmployee.DataTransferObject.Smtp;
using ManageEmployee.Entities.Email;
using System.Linq.Expressions;

namespace ManageEmployee.Services.Interfaces.Email
{
    public interface ISmtpService
    {
        Task<List<Smtp>> GetAll();
        Task<IEnumerable<Smtp>> GetMany(Expression<Func<Smtp, bool>> filter = null);
        Task<List<Smtp>> GetByListId(List<int> ids);
        Task<Smtp> GetById(int id);
        Task<Smtp> Create(SmtpModel model);
        Task<Smtp> Update(SmtpModel model);
        Task<bool> Delete(int id);
    }
}
