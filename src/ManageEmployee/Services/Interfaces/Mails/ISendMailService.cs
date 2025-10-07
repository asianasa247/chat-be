using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities;

namespace ManageEmployee.Services.Interfaces.Mails;

public interface ISendMailService
{
    Task<IEnumerable<SendMail>> GetAll(PagingRequestModel request);
    Task Create(SendMail request);
    Task Update(SendMail request);
    Task Delete(int id);
    void SendEmail(SendMail request, string customerEmail);
}
