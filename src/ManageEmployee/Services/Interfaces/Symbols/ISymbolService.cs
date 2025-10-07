using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.InOutEntities;

namespace ManageEmployee.Services.Interfaces.Symbols;

public interface ISymbolService
{
    IEnumerable<Symbol> GetAll();

    Task<PagingResult<Symbol>> GetAll(PagingRequestModel param);

    Symbol GetById(int id);

    Symbol Create(Symbol param);

    void Update(Symbol param);

    void Delete(int id);
}
