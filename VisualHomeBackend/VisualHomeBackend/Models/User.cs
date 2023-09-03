namespace VisualHomeBackend.Models
{
    public class User
    {
        public string Name { get; set; } = "";
        public string Password { get; set; } = "";
        public int RealtimeRateMs { get; set; } = 1000;
        public bool IsAdmin { get; set; } = false;
    }
}
