using ManageEmployee.DataTransferObject;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Extends;

public static class ResultExtension
{
    public static IActionResult Return<T>(this Result<T> result)
    {
        return result.IsSuccess 
            ? new OkObjectResult(result.Data)
            : new BadRequestObjectResult(result.Message);
    }

    public static IActionResult Return(this Result result)
    {
        return result.IsSuccess
            ? new OkResult()
            : new BadRequestObjectResult(result.Message);
    }

    public static IActionResult ReturnOk<T>(this Result<List<T>> result)
    {
        return new OkObjectResult(result.IsSuccess ? result.Data : new List<T>());
    }
}