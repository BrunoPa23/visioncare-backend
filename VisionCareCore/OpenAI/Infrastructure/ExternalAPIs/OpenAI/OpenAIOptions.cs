using System.ComponentModel.DataAnnotations;

namespace VisionCareCore.OpenAI.Infrastructure.ExternalAPIs.OpenAI;

/// <summary>
/// Configuracion de OpenAI (seccion OpenAI). Las claves van en user-secrets o variables de entorno.
/// </summary>
public class OpenAIOptions
{
    public const string Seccion = "OpenAI";

    [Required]
    public string ApiKey { get; set; } = string.Empty;

    [Required]
    public string AssistantId { get; set; } = string.Empty;
}
