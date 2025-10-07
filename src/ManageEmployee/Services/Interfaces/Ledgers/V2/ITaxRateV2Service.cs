using ManageEmployee.DataTransferObject.V2;

namespace ManageEmployee.Services.Interfaces.Ledgers.V2;

public interface ITaxRateV2Service
{
    Task<IEnumerable<TaxRateV2Model>> GetAll(int year);
}
