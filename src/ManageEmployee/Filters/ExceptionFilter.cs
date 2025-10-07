using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ManageEmployee.Filters;

public class ExceptionFilter: IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        var response = new ResponseWrapperFilterAttribute.GenericResponse<object>()
        {
            Message = $"{exception.Message}",
        };
        
        context.Result = new JsonResult(response)
        {
            StatusCode = 400
        };
    }
}