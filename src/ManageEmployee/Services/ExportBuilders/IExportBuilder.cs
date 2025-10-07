namespace ManageEmployee.Services.ExportBuilders;

public interface IExportBuilder
{
    Task<string> Export<T>(ExportRequestModel<T> request) where T : class;
}
