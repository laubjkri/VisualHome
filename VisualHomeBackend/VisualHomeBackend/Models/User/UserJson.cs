namespace VisualHomeBackend.Models.User
{
    public class UserJson
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? Password { get; set; }
        public int? RealtimeRateMs { get; set; }
        public bool? IsAdmin { get; set; }
    }
}
