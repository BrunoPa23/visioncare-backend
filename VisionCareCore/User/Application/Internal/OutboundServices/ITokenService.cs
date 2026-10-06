namespace VisionCareCore.User.Application.Internal.OutboundServices;

public interface ITokenService
{
    /// <summary>
    /// Generate a JWT access token
    /// </summary>
    /// <param name="user">The user to generate the token for</param>
    /// <returns>The generated access token</returns>
    string GenerateToken(Domain.Model.Aggregates.AuthUser user);

    /// <summary>
    /// Validate a JWT token
    /// </summary>
    /// <param name="token">The token to validate</param>
    /// <returns>The user id if the token is valid, null otherwise</returns>
    (Guid? UserId, string Role) ValidateToken(string token); // 🔥 Devuelve también el rol

    /// <summary>
    /// Generate a Refresh Token
    /// </summary>
    /// <returns>A new Refresh Token</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Store a Refresh Token for a user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="refreshToken">The refresh token</param>
    /// <returns>A task representing the async operation</returns>
    Task StoreRefreshToken(Guid userId, string refreshToken);

    /// <summary>
    /// Validate a Refresh Token
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="refreshToken">The refresh token</param>
    /// <returns>True if valid, otherwise false</returns>
    Task<Guid?> ValidateRefreshToken(Guid userId, string refreshToken);

    /// <summary>
    /// Remove a Refresh Token
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <returns>A task representing the async operation</returns>
    Task RevokeRefreshToken(string refreshToken);
}