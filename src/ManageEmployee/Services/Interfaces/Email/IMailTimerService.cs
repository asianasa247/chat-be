using ManageEmployee.DataTransferObject.Email;
using ManageEmployee.Entities.Email;
using System.Linq.Expressions;

namespace ManageEmployee.Services.Interfaces.Email
{
    public interface IMailTimerService
    {
        Task<List<MailTimer>> GetAll();
        Task<IEnumerable<MailTimer>> GetMany(Expression<Func<MailTimer, bool>> filter = null);
        Task<MailTimer> GetById(int id);
        Task<MailTimer> Create(MailTimerModel model);
        Task<MailTimer> Update(MailTimerModel model);
        Task<bool> Delete(int id);
    }
}
