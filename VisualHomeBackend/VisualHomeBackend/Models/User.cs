namespace VisualHomeBackend.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = "";
        public string Password { get; set; } = "";
        public int RealtimeRateMs { get; set; } = 1000;
        public bool IsAdmin { get; set; } = false;
    }
    
    public class UserJson
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Password { get; set; } = "";
        public int RealtimeRateMs { get; set; } = 1000;
        public bool IsAdmin { get; set; } = false;
    }
}
