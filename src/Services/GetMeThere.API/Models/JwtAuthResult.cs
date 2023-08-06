namespace GetMeThere.API.Models
{
    public class JwtAuthResult
    {
        public string? AccessToken { get; set; }
        public int ExpiresIn { get; set; }
        public RefreshToken? RefreshToken { get; set; }

    }
}
