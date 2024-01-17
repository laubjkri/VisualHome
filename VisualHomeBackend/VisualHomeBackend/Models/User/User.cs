namespace VisualHomeBackend.Models.User
{
    public class User
    {
        public User(Guid id, string name, string password, int realtimeRateMs, bool isAdmin)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Password = password ?? throw new ArgumentNullException(nameof(password));
            RealtimeRateMs = realtimeRateMs;
            IsAdmin = isAdmin;
        }

        public Guid Id { get; }
        public string Name { get; set; }
        public string Password { get; set; }

        private int _realtimeRateMs;
        public int RealtimeRateMs
        {
            get => _realtimeRateMs;
            set => _realtimeRateMs = ClampReamTimeRate(value);
        }

        public bool IsAdmin { get; set; }

        public int RealTimeRateMax { get; } = 30000;
        public int RealTimeRateMin { get; } = 500;

        private int ClampReamTimeRate(int input)
        {

            if (input > RealTimeRateMax)
            {
                input = RealTimeRateMax;
            }
            else if (input < RealTimeRateMin)
            {
                input = RealTimeRateMin;
            }
            return input;
        }

        public static User TryCreate(UserJson jsonUser)
        {
            if(jsonUser == null)
            {

            }

        }

    }
}
