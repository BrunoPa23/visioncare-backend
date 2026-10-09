namespace VisionCareCore.User.Infraestructure.Tokens.JWT.Configurations;

public class TokenSettings
{
    public string Secret { get; set; }
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}