using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ManageEmployee.Filters;

public class CustomHeaderSwaggerAttribute : IOperationFilter
{
    private readonly IConfiguration _configuration;
    public CustomHeaderSwaggerAttribute(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters == null)
        {
            operation.Parameters = new List<OpenApiParameter>();
        }
        
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "DbName",
            In = ParameterLocation.Header,
            Required = false,
            Schema = new OpenApiSchema { Type = "string", Default = new OpenApiString(_configuration.GetConnectionString("DbName")) },
            Description = "DbName"
        });
    }
}
