using Microsoft.EntityFrameworkCore;
using VisionCareCore.Shared.Infraestructure.Persistences.EFC.Configuration;
using VisionCareCore.Shared.Infraestructure.Persistences.EFC.Repositories;
using VisionCareCore.User.Domain.Model.Aggregates;
using VisionCareCore.User.Domain.Repositories;

namespace VisionCareCore.User.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
/// Repositorio de usuarios; la busqueda por id, el alta y el listado vienen de BaseRepository.
/// </summary>
public class AuthUserRepository(AppDbContext context) : BaseRepository<AuthUser>(context), IAuthUserRepository
{
    public async Task<AuthUser?> FindByEmailAsync(string email)
    {
        return await Context.Set<AuthUser>()
            .FirstOrDefaultAsync(user => user.Email.Equals(email));
    }

    public async Task<AuthUser?> FindByRefreshTokenAsync(string refreshToken)
    {
        return await Context.AuthUsers
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken && rt.ExpiryDate > DateTime.UtcNow));
    }

    public bool ExistsByEmail(string email)
    {
        return Context.Set<AuthUser>().Any(user => user.Email.Equals(email));
    }
}
