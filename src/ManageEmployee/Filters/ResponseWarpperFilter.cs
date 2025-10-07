using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ManageEmployee.Filters;

public class ResponseWrapperFilterAttribute: ActionFilterAttribute
{
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is ObjectResult objectResult)
        {
            var response = new GenericResponse<object>
            {
                Data = objectResult.Value,
                Success = true,
                Message = "Lấy dữ liệu thành công"
            };

            context.Result = new ObjectResult(response)
            {
                StatusCode = objectResult.StatusCode
            };
        }
        else if (context.Result is EmptyResult)
        {
            var response = new GenericResponse<object>
            {
                Success = true,
                Message = "Thất bại"
            };

            context.Result = new OkObjectResult(response);
        }
    }

    public class GenericResponse<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}