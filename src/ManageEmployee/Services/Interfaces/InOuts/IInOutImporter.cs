namespace ManageEmployee.Services.Interfaces.InOuts;

public interface IInOutImporter
{
    Task ImportAsync(IFormFile file);
}
