namespace VisionCareCore.Shared.Infraestructure.Interfaces.ASP.Configuration.RateLimiting;

/// <summary>
/// Nombres de las politicas de rate limiting usadas en [EnableRateLimiting].
/// </summary>
public static class RateLimitPolicies
{
    public const string Vision = "vision";
    public const string Gpt = "gpt";
    public const string SignIn = "sign-in";
}
