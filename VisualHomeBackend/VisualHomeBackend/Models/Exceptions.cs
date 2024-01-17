namespace VisualHomeBackend.Models
{
    // A collection of exceptions of which most has an equivalent HTTP response code.


    public class NotFoundException(string? message, Exception? innerException) : Exception(message, innerException) {}
    public class HttpResponseException(string? message, Exception? innerException) : Exception(message, innerException) { }



}
