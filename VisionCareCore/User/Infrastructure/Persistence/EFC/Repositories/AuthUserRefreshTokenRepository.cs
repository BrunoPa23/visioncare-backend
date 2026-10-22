using Microsoft.EntityFrameworkCore;
using VisionCareCore.Shared.Infrastructure.Persistence.EFC.Configuration;
using VisionCareCore.Shared.Infrastructure.Persistence.EFC.Repositories;
using VisionCareCore.User.Domain.Model.Aggregates;
using VisionCareCore.User.Domain.Repositories;

namespace VisionCareCore.User.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
/// Repositorio de refresh tokens; AddAsync y UpdateAsync (que guardan al instante) vienen de BaseRepository.
/// </summary>
public class AuthUserRefreshTokenRepository(AppDbContext context)
    : BaseRepository<AuthUserRefreshToken>(context), IAuthUserRefreshTokenRepository
{
    public async Task<AuthUserRefreshToken?> GetByTokenAsync(string token) =>
        await Context.AuthUsersRefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);

    public async Task<AuthUserRefreshToken?> GetByUserIdAsync(Guid userId) =>
        await Context.AuthUsersRefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == userId);

    public async Task RevokeAsync(AuthUserRefreshToken refreshToken)
    {
        refreshToken.IsRevoked = true;
        await UpdateAsync(refreshToken);
    }
}
