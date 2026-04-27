namespace Mobile.Shared.Models;

public class LoginRequest
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}

public class RegisterRequest
{
    public string? Login { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
}

public class RefreshRequest
{
    public string? RefreshToken { get; set; }
}

public class AuthResultDto
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}

public class UserDto
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public string? Login { get; set; }
    public string? Name { get; set; }
}
