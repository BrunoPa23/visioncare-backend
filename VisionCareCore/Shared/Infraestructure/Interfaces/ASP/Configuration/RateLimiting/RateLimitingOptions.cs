using System.ComponentModel.DataAnnotations;

namespace VisionCareCore.Shared.Infraestructure.Interfaces.ASP.Configuration.RateLimiting;

/// <summary>
/// Limites de peticiones por ventana fija, configurables en la seccion RateLimiting.
/// </summary>
public class RateLimitingOptions
{
    public const string Seccion = "RateLimiting";

    // Escaneo y OCR: cada peticion llama a Azure AI Vision y a OpenAI
    [Required] public LimiteVentana Vision { get; set; } = new() { PermitLimit = 10, WindowSeconds = 60 };

    // Asistente de OpenAI
    [Required] public LimiteVentana Gpt { get; set; } = new() { PermitLimit = 20, WindowSeconds = 60 };

    // Inicio de sesion por IP, para frenar ataques de fuerza bruta
    [Required] public LimiteVentana SignIn { get; set; } = new() { PermitLimit = 5, WindowSeconds = 60 };
}

public class LimiteVentana
{
    [Range(1, 100_000)] public int PermitLimit { get; set; }

    [Range(1, 86_400)] public int WindowSeconds { get; set; }
}
