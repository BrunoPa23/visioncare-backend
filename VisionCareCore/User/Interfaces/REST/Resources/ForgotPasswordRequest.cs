namespace VisionCareCore.User.Interfaces.REST.Resources;

public record ForgotPasswordRequest
{
    public string Email { get; set; }
    public string Subject { get; set; }
    public string Message { get; set; }
}