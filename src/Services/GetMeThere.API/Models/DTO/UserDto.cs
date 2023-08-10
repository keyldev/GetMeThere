namespace GetMeThere.API.Models.DTO
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? Login { get; set; }
        public string? Name { get; set; }
    }
}
