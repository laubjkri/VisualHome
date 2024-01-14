namespace VisualHomeBackend.Models
{
    public class User
    {
        public Guid? Id { get; init; }
        public string? Name { get; set; }
        public string? Password { get; set; }
        public int? RealtimeRateMs { get; set; }
        public bool? IsAdmin { get; set; }


        public void CopySetProperties(User other)
        {
            if (other.Name is not null)
                this.Name = other.Name;
            if (other.Password is not null)
                this.Password = other.Password;
            if (other.RealtimeRateMs is not null)
                this.RealtimeRateMs = other.RealtimeRateMs;
            if (other.IsAdmin is not null)
                this.IsAdmin = other.IsAdmin;
        }

    }
}
