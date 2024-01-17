using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using System.Net;
using VisualHomeBackend.Models.User;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace VisualHomeBackend.Services
{
    public class ExceptionHandler : IExceptionHandler
    {
        private readonly ILogger _logger;
        public ExceptionHandler(ILogger logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            string message = "";
            httpContext.Response.


            switch (exception)
            {
                case FailedToUpdateDbException ex:
                    message = "Failed to update user in database.";
                    break;

                default:
                    message = exception.Message;
                    break;
            }

            
            await httpContext.Response.WriteAsync(message, cancellationToken);

            return true;
        }
    }
}
