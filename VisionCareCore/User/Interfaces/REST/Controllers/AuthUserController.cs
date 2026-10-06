using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VisionCareCore.User.Domain.Model.Aggregates;
using VisionCareCore.User.Domain.Model.Queries;
using VisionCareCore.User.Domain.Services;
using VisionCareCore.User.Interfaces.REST.Transform;

namespace VisionCareCore.User.Interfaces.REST.Controllers;

[Authorize]
[ApiController]
[Route("vc/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class AuthUserController(IAuthUserQueryService authUserQueryService, IAuthUserCommandService userCommandService) : ControllerBase
{
    /**
     * <summary>
     *     Get user by id endpoint. It allows to get a user by id
     * </summary>
     * <param name="authUserId">The user id</param>
     * <returns>The user resource</returns>
     */
    [HttpGet("{authUserId}")]
    public async Task<IActionResult> GetAuthUserById(Guid authUserId)
    {
        var getAuthUserByIdQuery = new GetAuthUserByIdQuery(authUserId);
        var user = await authUserQueryService.Handle(getAuthUserByIdQuery);
        var userResource = AuthUserResourceFromEntityAssembler.ToResourceFromEntity(user!);
        return Ok(userResource);
    }

    /**
     * <summary>
     *     Get all users endpoint. It allows to get all users
     * </summary>
     * <returns>The user resources</returns>
     */
    [HttpGet]
    public async Task<IActionResult> GetAuthAllUsers()
    {
        var getAuthAllUsersQuery = new GetAllAuthUsersQuery();
        var users = await authUserQueryService.Handle(getAuthAllUsersQuery);
        var userResources = users.Select(AuthUserResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(userResources);
    }
    
    [HttpGet("me")]
    public async Task<IActionResult> GetAuthenticatedUser()
    {
        if (!HttpContext.User.Identity.IsAuthenticated)
        {
            Console.WriteLine("❌ Usuario no autenticado.");
            return Unauthorized(new { message = "Usuario no autenticado" });
        }

        Console.WriteLine("✅ Usuario autenticado correctamente.");

        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
                          HttpContext.User.FindFirst("sub")?.Value;  
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized(new { message = "No se encontró el ID del usuario" });
        }

        var userId = Guid.Parse(userIdClaim);
        var user = await authUserQueryService.Handle(new GetAuthUserByIdQuery(userId));

        return Ok(new
        {
            id = user.Id,
            email = user.Email,
            name = user.Name,
            visualImpairment = user.VisualImpairment.ToString()
        });
    }



    
    
}