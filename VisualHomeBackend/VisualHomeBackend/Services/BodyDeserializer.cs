using System.Text.Json;
using VisualHomeBackend.Models;

namespace VisualHomeBackend.Services
{
    public class BodyDeserializer
    {
        private static JsonSerializerOptions JsonSerializerOptions { get; } = new JsonSerializerOptions()
        {
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
