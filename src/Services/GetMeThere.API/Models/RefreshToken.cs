namespace GetMeThere.API.Models
{
    public class RefreshToken
    {

        public int TokenId { get; set; }
        public Guid UserId { get; set; }
        public string? TokenString { get; set; }
        public DateTime ExpiryTime { get; set; }

    }
}
