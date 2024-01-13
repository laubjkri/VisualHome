
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace VisualHomeBackend.Extensions
{
    public class Locked : IResult
    {
        private readonly string _message;

        public Locked(string message)
        {
            _message = message;
        }

        public async Task ExecuteAsync(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = StatusCodes.Status423Locked;
            //httpContext.Response.ContentType = "application/json";            
            await httpContext.Response.WriteAsync(_message);
        }

    }

    static partial class ResultsExtensions
    {
        public static IResult Locked(this IResultExtensions resultExtensions, string message)
        {
            ArgumentNullException.ThrowIfNull(resultExtensions);

            return new Locked(message);
        }
    }

}
