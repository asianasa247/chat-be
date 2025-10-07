using ManageEmployee.DataTransferObject.Email;
using ManageEmployee.Entities.Email;
using System.Linq.Expressions;

namespace ManageEmployee.Services.Interfaces.Email
{
    public interface IMailTemplateService
    {
        Task<List<MailTemplate>> GetAll();
        Task<IEnumerable<MailTemplate>> GetMany(Expression<Func<MailTemplate, bool>> filter = null);
        Task<List<MailTemplate>> GetByListId(List<int> ids);
        Task<MailTemplate> GetById(int id);
        Task<MailTemplate> Create(MailTemplateModel model);
        Task<MailTemplate> Update(MailTemplateModel model);
        Task<bool> Delete(int id);
    }
}
