using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LabTest.Api.Helpers
{
    public class GlobalExceptionFilter: IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(0, context.Exception, context.Exception.Message);
            context.ExceptionHandled = true;
            var arr = new List<string>();
            var ex = context.Exception;
            while (ex != null)
            {
                arr.Add($"Message: {ex.Message}");
                ex = ex.InnerException;
            }
            context.Result = new ObjectResult(arr) { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }
}
