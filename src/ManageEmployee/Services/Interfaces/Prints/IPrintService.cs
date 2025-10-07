using ManageEmployee.DataTransferObject.PrintViewModelModels;
using ManageEmployee.Entities.PrintEntities;

namespace ManageEmployee.Services.Interfaces.Prints;

public interface IPrintService
{
    IEnumerable<Print> GetAll();

    PagePrintViewModel GetPagePrint();

    Print Create(Print param);

    void Update(PagePrintViewModel param);
}
