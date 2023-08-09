namespace GetMeThere.API.Models
{
    public class RegisterRequest
    {

        public string? Login { get; set; } // login = username
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }


    }
}
