using ManageEmployee.Entities.Chatbot;

namespace ManageEmployee.Services.Interfaces.Chatbot
{
    public interface ICompanyInfoService
    {
        Task<CompanyInfo> LoadAsync(CancellationToken ct = default);
        string? QuickAnswer(CompanyInfo info, string? userText);
    }
}
