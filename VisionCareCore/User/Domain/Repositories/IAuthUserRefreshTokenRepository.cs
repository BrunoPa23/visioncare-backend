using VisionCareCore.Shared.Domain.Repositories;
using VisionCareCore.User.Domain.Model.Aggregates;

namespace VisionCareCore.User.Domain.Repositories;

public interface IAuthUserRefreshTokenRepository : IBaseRepository<AuthUserRefreshToken>
{
    Task<AuthUserRefreshToken?> GetByTokenAsync(string token);
    Task<AuthUserRefreshToken?> GetByUserIdAsync(Guid userId);
    Task RevokeAsync(AuthUserRefreshToken refreshToken);
}
