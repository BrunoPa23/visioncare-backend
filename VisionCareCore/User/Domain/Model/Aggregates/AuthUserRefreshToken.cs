namespace VisionCareCore.User.Domain.Model.Aggregates;

public class AuthUserRefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; } 
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; } 
    public bool IsRevoked { get; set; } = false; 
    public AuthUser AuthUser { get; set; } 

}