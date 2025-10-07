using ManageEmployee.DataTransferObject.InOutModels;

namespace ManageEmployee.Services.Interfaces.InOuts
{
    public interface IHanetInOut
    {
        Task SetDataLogIn(HanetModel form);
        Task SetDataLogOut(HanetModel form);
    }
}
