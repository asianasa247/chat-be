using ManageEmployee.DataTransferObject;
using ManageEmployee.Entities;

namespace ManageEmployee.Services.Interfaces.TaxRates;

public interface ITaxRateService
{
    Task<IEnumerable<TaxRate>> GetAll();

    Task<List<TaxRate>> GetPage(int currentPage, int pageSize);

    Task<int> CountAll();

    Task<string> Create(TaxRate entity);

    Task<string> Update(TaxRate entity);

    Task<string> Delete(long id);

    Task<TaxRate> GetTaxTypeByCode(string code);

    Task<List<TaxRate>> GetListTaxRateStartWithR();
    Task<TaxRateModel> GetById(long id, int year);
}
