namespace GetMeThere.API.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Login { get; set; } = "";
        public string Password { get; set; } = "";
        public string Name { get; set; } = "";
    }
}
