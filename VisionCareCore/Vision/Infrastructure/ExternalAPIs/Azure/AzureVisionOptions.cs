using System.ComponentModel.DataAnnotations;

namespace VisionCareCore.Vision.Infrastructure.ExternalAPIs.Azure;

/// <summary>
/// Configuracion de Azure AI Vision (seccion AzureVision). Las claves van en user-secrets o variables de entorno.
/// </summary>
public class AzureVisionOptions
{
    public const string Seccion = "AzureVision";

    [Required, Url]
    public string Endpoint { get; set; } = string.Empty;

    [Required]
    public string Key { get; set; } = string.Empty;
}
