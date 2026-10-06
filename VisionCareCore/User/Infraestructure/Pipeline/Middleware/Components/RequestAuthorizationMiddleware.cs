using Microsoft.AspNetCore.Authorization;
using VisionCareCore.User.Application.Internal.OutboundServices;
using VisionCareCore.User.Domain.Model.Queries;
using VisionCareCore.User.Domain.Services;

namespace VisionCareCore.User.Infraestructure.Pipeline.Middleware.Components;

public class RequestAuthorizationMiddleware(RequestDelegate next)
{
    /**
     * InvokeAsync is called by the ASP.NET Core runtime.
     * It is used to authorize requests.
     * It validates a token is included in the request header and that the token is valid.
     * If the token is valid then it sets the user in HttpContext.Items["User"].
     */
    public async Task InvokeAsync(
        HttpContext context,
        IAuthUserQueryService userQueryService,
        ITokenService tokenService)
    {
        Console.WriteLine("Entering InvokeAsync");

        var allowAnonymous = context.Request.HttpContext.GetEndpoint()!.Metadata
            .Any(m => m.GetType() == typeof(AllowAnonymousAttribute));

        if (allowAnonymous)
        {
            Console.WriteLine("Skipping authorization");
            await next(context);
            return;
        }

        // 🔹 Obtener el token desde la cookie HttpOnly
        var token = context.Request.Cookies["AuthToken"];

        if (string.IsNullOrEmpty(token))
        {
            throw new Exception("Token no encontrado en la cookie");
        }

        // 🔹 Desestructurar correctamente la tupla (UserId, Role)
        var (userId, role) = tokenService.ValidateToken(token);

        if (!userId.HasValue) // ✅ Verifica si el Guid? tiene valor
        {
            throw new Exception("Token inválido");
        }

        var getUserByIdQuery = new GetAuthUserByIdQuery(userId.Value); // Usa .Value para acceder al GUID
        var user = await userQueryService.Handle(getUserByIdQuery);

        if (user == null)
        {
            throw new Exception("Usuario no encontrado.");
        }

        Console.WriteLine("Autorización exitosa. Guardando usuario y rol en contexto...");

        // 🔹 Guardamos tanto el usuario como el rol en el contexto HTTP
        context.Items["User"] = user;
        context.Items["UserRole"] = role;

        Console.WriteLine("Continuando con el Middleware Pipeline...");
        await next(context);
    }

    
}