using System.Globalization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using VisionCareCore.Shared.Infraestructure.Interfaces.ASP.Extensions;

namespace VisionCareCore.Shared.Infraestructure.Interfaces.ASP.Configuration.RateLimiting;

public static class RateLimitingExtensions
{
    /// <summary>
    /// Registra las politicas de vision, GPT y sign-in con ventana fija.
    /// Vision y GPT se limitan por usuario autenticado (o por IP si no hay usuario); sign-in siempre por IP.
    /// Al superar el limite responde 429 con ProblemDetails y la cabecera Retry-After.
    /// </summary>
    public static IServiceCollection AddVisionCareRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<RateLimitingOptions>()
            .Bind(configuration.GetSection(RateLimitingOptions.Seccion))
            .ValidateDataAnnotations()
            .Validate(o => EsValido(o.Vision) && EsValido(o.Gpt) && EsValido(o.SignIn),
                "Cada limite de RateLimiting necesita PermitLimit y WindowSeconds mayores a 0")
            .ValidateOnStart();

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddPolicy(RateLimitPolicies.Vision, context =>
                VentanaFija(context, ClavePorUsuario(context), o => o.Vision));
            options.AddPolicy(RateLimitPolicies.Gpt, context =>
                VentanaFija(context, ClavePorUsuario(context), o => o.Gpt));
            options.AddPolicy(RateLimitPolicies.SignIn, context =>
                VentanaFija(context, ClavePorIp(context), o => o.SignIn));

            options.OnRejected = EscribirRechazoAsync;
        });

        return services;
    }

    private static bool EsValido(LimiteVentana limite) => limite.PermitLimit > 0 && limite.WindowSeconds > 0;

    private static RateLimitPartition<string> VentanaFija(
        HttpContext context, string clave, Func<RateLimitingOptions, LimiteVentana> limite)
    {
        var opciones = limite(context.RequestServices.GetRequiredService<IOptions<RateLimitingOptions>>().Value);
        return RateLimitPartition.GetFixedWindowLimiter(clave, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = opciones.PermitLimit,
            Window = TimeSpan.FromSeconds(opciones.WindowSeconds),
            QueueLimit = 0
        });
    }

    private static string ClavePorUsuario(HttpContext context) =>
        context.User.GetUserId() is { } userId ? $"usuario:{userId}" : ClavePorIp(context);

    // Detras de un proxy hay que configurar ForwardedHeaders para que la IP sea la del cliente
    private static string ClavePorIp(HttpContext context) =>
        $"ip:{context.Connection.RemoteIpAddress?.ToString() ?? "desconocida"}";

    private static async ValueTask EscribirRechazoAsync(OnRejectedContext rechazo, CancellationToken cancellationToken)
    {
        var http = rechazo.HttpContext;
        var segundos = rechazo.Lease.TryGetMetadata(MetadataName.RetryAfter, out var espera)
            ? (int)Math.Ceiling(espera.TotalSeconds)
            : (int?)null;
        if (segundos is not null)
        {
            http.Response.Headers.RetryAfter = segundos.Value.ToString(CultureInfo.InvariantCulture);
        }

        http.RequestServices.GetRequiredService<ILoggerFactory>()
            .CreateLogger(typeof(RateLimitingExtensions))
            .LogWarning("Limite de peticiones superado en {Method} {Path}", http.Request.Method, http.Request.Path);

        http.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await http.RequestServices.GetRequiredService<IProblemDetailsService>().WriteAsync(new ProblemDetailsContext
        {
            HttpContext = http,
            ProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status429TooManyRequests,
                Title = "Demasiadas peticiones",
                Detail = segundos is null
                    ? "Se supero el limite de peticiones. Intenta de nuevo mas tarde."
                    : $"Se supero el limite de peticiones. Intenta de nuevo en {segundos} segundos."
            }
        });
    }
}
