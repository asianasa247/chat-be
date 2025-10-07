using ManageEmployee.Entities;

namespace ManageEmployee.Services.Interfaces
{
    public interface IFaceAccessService
    {
        FaceAccess GetActiveAccess();
        Task<bool> AddAccessAsync(FaceAccess access);
        Task<FaceAccess?> GetAccessByIdAsync(Guid accessId);
        Task<bool> UpdateAccessAsync(FaceAccess access);
        Task<IEnumerable<FaceAccess>> GetAllAsync();
    }
}
