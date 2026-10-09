using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VisionCareCore.User.Application.Internal.OutboundServices;
using VisionCareCore.User.Domain.Model.Aggregates;
using VisionCareCore.User.Domain.Model.Commands;
using VisionCareCore.User.Domain.Repositories;
using VisionCareCore.User.Domain.Services;
using VisionCareCore.User.Interfaces.REST.Resources;
using VisionCareCore.User.Interfaces.REST.Transform;

namespace VisionCareCore.User.Interfaces.REST.Controllers;

[Authorize]
[ApiController]
[Route("vc/v1/authentication")]
[Produces(MediaTypeNames.Application.Json)]
public class AuthenticationController(
    IAuthUserCommandService userCommandService,
    IAuthUserRepository authUserRepository,
    ITokenService tokenService) : ControllerBase
{
    [HttpPost("sign-in")]
    [AllowAnonymous]
    public async Task<IActionResult> SignIn([FromBody] SignInResource signInResource)
    {
        var user = await authUserRepository.FindByEmailAsync(signInResource.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(signInResource.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Usuario o contraseña incorrectos" });
        }

        //  if (!user.IsActive)
        //  {
        //      return Unauthorized(new { message = "Tu cuenta aún no ha sido activada." });
        //  }

        var jwtToken = tokenService.GenerateToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();

        await tokenService.StoreRefreshToken(user.Id, refreshToken);

        Response.Cookies.Append("AuthToken", jwtToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddMinutes(30)
        });

        Response.Cookies.Append("RefreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddDays(7)
        });

        return Ok(new { message = "Inicio de sesión exitoso" });
    }

    [HttpPost("sign-up")]
    [AllowAnonymous]
    public async Task<IActionResult> SignUp([FromBody] SignUpResource signUpResource)
    {
        var existingUser = await authUserRepository.FindByEmailAsync(signUpResource.Email);
        if (existingUser != null)
        {
            return BadRequest(new { message = "El usuario ya existe" });
        }

        var signUpCommand = SignUpCommandFromResourceAssembler.ToCommand(signUpResource);
        if (signUpCommand is null)
        {
            return BadRequest(new { message = "Nivel de discapacidad visual inválido" });
        }

        await userCommandService.Handle(signUpCommand);

        return Ok(new { message = "Usuario creado exitosamente" });
    }
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["RefreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized(new { message = "No hay refresh token disponible" });
        }

        var userId = await tokenService.ValidateRefreshToken(refreshToken);
        if (userId == null)
        {
            return Unauthorized(new { message = "Refresh token inválido o expirado" });
        }

        var user = await authUserRepository.FindByIdAsync(userId.Value);
        if (user == null)
        {
            return Unauthorized(new { message = "Usuario no encontrado" });
        }

        var newJwtToken = tokenService.GenerateToken(user);
        var newRefreshToken = tokenService.GenerateRefreshToken();

        await tokenService.StoreRefreshToken(user.Id, newRefreshToken);

        Response.Cookies.Append("AuthToken", newJwtToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddMinutes(30)
        });

        Response.Cookies.Append("RefreshToken", newRefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddDays(7)
        });

        return Ok(new { message = "Token renovado exitosamente" });
    }

    [HttpPost("sign-out")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["RefreshToken"];

        if (!string.IsNullOrEmpty(refreshToken))
        {
            await tokenService.RevokeRefreshToken(refreshToken);
        }

        Response.Cookies.Delete("AuthToken");
        Response.Cookies.Delete("RefreshToken");

        return Ok(new { message = "Logout exitoso" });
    }
}
