using ManageEmployee.DataTransferObject.V2;

namespace ManageEmployee.Services.Interfaces.Ledgers.V2;

public interface IDescriptionV2Service
{
    Task<IEnumerable<DescriptionV2Model>> GetPage(int year, string documentCode);
}
