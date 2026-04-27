namespace GetMeThere.API.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Login { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string Name { get; set; } = "";
        public string? PhoneNumber { get; set; }
        public string? FcmDeviceToken { get; set; }

        public string? ConnectionId { get; set; }
    }
}
