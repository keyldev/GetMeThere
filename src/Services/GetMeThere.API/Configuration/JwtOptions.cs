namespace GetMeThere.API.Configuration;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Key { get; init; } = "";
    public string Issuer { get; init; } = "";
    public string Audience { get; init; } = "";

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Key) || Key.Length < 32)
            throw new InvalidOperationException(
                "Jwt:Key is missing or too short (>=32 chars). Set via user-secrets or env var GMT_Jwt__Key.");
        if (string.IsNullOrWhiteSpace(Issuer))
            throw new InvalidOperationException("Jwt:Issuer is missing.");
        if (string.IsNullOrWhiteSpace(Audience))
            throw new InvalidOperationException("Jwt:Audience is missing.");
    }
}
