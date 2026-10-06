namespace VisionCareCore.User.Domain.Model.Commands
{
    public class LogoutRequest
    {
        public string RefreshToken { get; set; }
    }
}