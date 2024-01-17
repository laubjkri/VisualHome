using System.Text.Json;
using VisualHomeBackend.Models;

namespace VisualHomeBackend.Services
{
    public class BodyDeserializer
    {
        private static JsonSerializerOptions JsonSerializerOptions { get; } = new JsonSerializerOptions()
        {
            // If a member is passed that is not in the type, then we fail the parsing so we know something is wrong.
            UnmappedMemberHandling = System.Text.Json.Serialization.JsonUnmappedMemberHandling.Disallow             
        };


        public static async Task<T?> GetObject<T>(HttpRequest httpRequest)
        {
            string requestBody;
            using (var reader = new StreamReader(httpRequest.Body))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            try
            {
                T? deserializedObject = JsonSerializer.Deserialize<T>(requestBody, JsonSerializerOptions);
                return deserializedObject;
            }
            catch (Exception)
            {
                return default(T);
            }            
        }
    }
}
