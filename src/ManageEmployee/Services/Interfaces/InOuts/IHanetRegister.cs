
namespace ManageEmployee.Services.Interfaces.InOuts
{
    public interface IHanetRegister
    {
        Task RegisterFace(IEnumerable<int> userIds, int placeId);
    }
}
