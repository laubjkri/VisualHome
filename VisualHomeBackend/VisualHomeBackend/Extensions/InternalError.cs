
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace VisualHomeBackend.Extensions
{
    public class InternalError : IResult
    {
        private readonly string _message;

        public InternalError(string message)
        {
            _message = message;
        }

        public async Task ExecuteAsync(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            //httpContext.Response.ContentType = "application/json";            
            await httpContext.Response.WriteAsync(_message);
        }

    }

    static partial class ResultsExtensions
    {
        public static IResult InternalError(this IResultExtensions resultExtensions, string message)
        {
            ArgumentNullException.ThrowIfNull(resultExtensions);

            return new InternalError(message);
        }
    }

}
