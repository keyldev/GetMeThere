namespace GetMeThere.API.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Login { get; set; } = "";
        public string Password { get; set; } = "";
        public string Name { get; set; } = "";

        public string ConnectionId { get; set; }

        public User()
        {

        }
        public User(RegisterRequest user)
        {
            this.Login = user.Login;
            this.Password = user.Password;
            this.Name = user.Name;
        }
    }
}
