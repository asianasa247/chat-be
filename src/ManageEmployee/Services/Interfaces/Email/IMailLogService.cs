using ManageEmployee.DataTransferObject.Email;
using ManageEmployee.Entities.Email;
using System.Linq.Expressions;

namespace ManageEmployee.Services.Interfaces.Email
{
    public interface IMailLogService
    {
        Task<List<MailLog>> GetAll();
        Task<IEnumerable<MailLog>> GetMany(Expression<Func<MailLog, bool>> filter = null);
        Task<MailLog> GetById(int id);
        Task<MailLog> Create(MailLogModel model);
        Task<MailLog> Update(MailLogModel model);
        Task<bool> Delete(int id);
    }
}
