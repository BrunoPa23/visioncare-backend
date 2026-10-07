using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace VisionCareCore.Shared.Infraestructure.Interfaces.ASP.Extensions;

/// <summary>
/// Lee el id del usuario autenticado desde los claims del token JWT.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal user)
    {
        // JwtBearer mapea "sub" a NameIdentifier por defecto; se revisan ambos por si cambia esa configuracion
        var valor = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);
        return Guid.TryParse(valor, out var userId) ? userId : null;
    }
}
