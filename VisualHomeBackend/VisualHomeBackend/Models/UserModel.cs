namespace VisualHomeBackend.Models
{
    public class UserModel
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public int WebSocketUpdateRateMs { get; set; } = 1000;
        public bool CanEditUsers { get; set; } = false;
    }
}
