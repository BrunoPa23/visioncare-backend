namespace VisionCareCore.User.Interfaces.REST.Resources;

public class ResetPasswordRequest
{
    public string Token { get; set; }
    public string NewPassword { get; set; }
}