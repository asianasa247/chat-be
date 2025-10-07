namespace ManageEmployee.Services.Interfaces.Users;

public interface IUserContractService
{
    Task<string> DownloadContract(int userId, int contractFileId);
}
