using System.Text;

namespace ManageEmployee.Services.Interfaces.Generators;

public interface IPdfGeneratorService
{
    Task<byte[]> GeneratePdf(string html, string type);
    Task MappingCompany(StringBuilder sb);
}
