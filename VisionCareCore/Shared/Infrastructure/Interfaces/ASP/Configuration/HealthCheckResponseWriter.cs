using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace VisionCareCore.Shared.Infrastructure.Interfaces.ASP.Configuration;

/// <summary>
/// Respuesta JSON de /health con el estado general y el de cada verificacion.
/// No incluye mensajes de excepcion ni datos de conexion.
/// </summary>
public static class HealthCheckResponseWriter
{
    private static readonly JsonSerializerOptions Opciones = new(JsonSerializerDefaults.Web);

    public static Task EscribirAsync(HttpContext context, HealthReport reporte)
    {
        var cuerpo = new
        {
            status = reporte.Status.ToString(),
            duracionMs = Math.Round(reporte.TotalDuration.TotalMilliseconds, 1),
            checks = reporte.Entries.Select(e => new
            {
                nombre = e.Key,
                status = e.Value.Status.ToString(),
                duracionMs = Math.Round(e.Value.Duration.TotalMilliseconds, 1)
            })
        };

        context.Response.ContentType = "application/json";
        return context.Response.WriteAsync(JsonSerializer.Serialize(cuerpo, Opciones));
    }
}
