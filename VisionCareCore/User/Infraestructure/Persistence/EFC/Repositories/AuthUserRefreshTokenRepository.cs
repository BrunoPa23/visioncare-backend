using Microsoft.EntityFrameworkCore;
using VisionCareCore.Shared.Infraestructure.Persistences.EFC.Configuration;
using VisionCareCore.User.Domain.Model.Aggregates;
using VisionCareCore.User.Domain.Repositories;

namespace VisionCareCore.User.Infraestructure.Persistence.EFC.Repositories;

public class AuthUserRefreshTokenRepository : IAuthUserRefreshTokenRepository
{
    private readonly AppDbContext _context;

    public AuthUserRefreshTokenRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AuthUserRefreshToken?> GetByTokenAsync(string token) =>
        await _context.AuthUsersRefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);

    public async Task<AuthUserRefreshToken?> GetByUserIdAsync(Guid userId) =>
        await _context.AuthUsersRefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == userId);

    public async Task AddAsync(AuthUserRefreshToken refreshToken)
    {
        await _context.AuthUsersRefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task RevokeAsync(AuthUserRefreshToken refreshToken)
    {
        refreshToken.IsRevoked = true;
        _context.AuthUsersRefreshTokens.Update(refreshToken);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateAsync(AuthUserRefreshToken refreshToken)
    {
        _context.AuthUsersRefreshTokens.Update(refreshToken);
        await _context.SaveChangesAsync();
    }

}