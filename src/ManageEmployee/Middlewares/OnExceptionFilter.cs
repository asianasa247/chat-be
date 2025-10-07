using ManageEmployee.Helpers;
using ManageEmployee.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace ManageEmployee.Middlewares;

public class OnExceptionFilter : IExceptionFilter
{
    private readonly ILogger<OnExceptionFilter> _logger;

    public OnExceptionFilter(ILogger<OnExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        context.HttpContext.Request.Headers.TryGetValue("X-RequestId", out var requestId);
        if (context.Exception is ErrorException)
            HandleCustomError(context, requestId.ToString());
        else
            HandleStandardError(context, requestId.ToString());
    }

    private void HandleCustomError(ExceptionContext context, string requestId)
    {
        var error = context.Exception as ErrorException;

        if(error == null)
        {
            return;
        }

        var response = new Response<IEnumerable<string>>
        {
            Success = false,
            Message = error.Message,
            Messages = error.Messages,
            Code = error.HttpStatusCode,
            RequestId = requestId
        };

        _logger.LogError($"{requestId} {JsonConvert.SerializeObject(response)}");

        context.HttpContext.Response.Headers.Clear();
        context.HttpContext.Response.StatusCode = error.HttpStatusCode;
        context.Result = new JsonResult(response);
    }

    private void HandleStandardError(ExceptionContext context, string requestId)
    {
        _logger.LogError($"{requestId} {JsonConvert.SerializeObject(context.Exception)}");
    }
}