using ManageEmployee.DataTransferObject.Email;
using ManageEmployee.Entities.Email;
using System.Linq.Expressions;

namespace ManageEmployee.Services.Interfaces.Email
{
    public interface IMailSendService
    {
        Task<List<MailSend>> GetAll();
        Task<IEnumerable<MailSend>> GetMany(Expression<Func<MailSend, bool>> filter = null);
        Task<MailSend> GetByEmail(string email);
        Task<MailSend> GetById(int id);
        Task<MailSend> Create(MailSendModel model);
        Task<MailSend> Update(MailSendModel model);
        Task<bool> Delete(int id);
    }
}
