
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace VisualHomeBackend.Extensions
{
    public class UnauthorizedResponse : IResult
    {
        private readonly string _message;

        public UnauthorizedResponse(string message)
        {
            _message = message;
        }

        public async Task ExecuteAsync(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //httpContext.Response.ContentType = "application/json";            
            await httpContext.Response.WriteAsync(_message);
        }

    }

    static partial class ResultsExtensions
    {
        public static IResult UnauthorizedResponse(this IResultExtensions resultExtensions, string message)
        {
            ArgumentNullException.ThrowIfNull(resultExtensions);

            return new UnauthorizedResponse(message);
        }
    }

}
