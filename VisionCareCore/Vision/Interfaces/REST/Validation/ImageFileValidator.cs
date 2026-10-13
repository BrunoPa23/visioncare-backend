namespace VisionCareCore.Vision.Interfaces.REST.Validation;

/// <summary>
/// Valida las imagenes que se envian a Azure AI Vision: tamano, tipo declarado y firma real del archivo.
/// </summary>
public static class ImageFileValidator
{
    public const long MaxBytes = 10 * 1024 * 1024;

    // Margen para los encabezados del multipart sobre el tamano maximo de la imagen
    public const long MaxRequestBytes = MaxBytes + 64 * 1024;

    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/webp"
    };

    /// <summary>Devuelve el motivo del rechazo, o null si la imagen es valida.</summary>
    public static async Task<string?> ValidateAsync(IFormFile? file)
    {
        if (file is null || file.Length == 0)
            return "Debe enviar una imagen.";

        if (file.Length > MaxBytes)
            return $"La imagen supera el maximo de {MaxBytes / (1024 * 1024)} MB.";

        if (!AllowedContentTypes.Contains(file.ContentType))
            return "Formato no permitido. Use JPEG, PNG o WEBP.";

        // El Content-Type lo declara el cliente; se confirma con los primeros bytes del archivo
        var header = new byte[12];
        await using var stream = file.OpenReadStream();
        var read = await stream.ReadAtLeastAsync(header, header.Length, throwOnEndOfStream: false);

        return HasImageSignature(header.AsSpan(0, read)) ? null : "El archivo no es una imagen valida.";
    }

    private static bool HasImageSignature(ReadOnlySpan<byte> h)
    {
        var jpeg = h.Length >= 3 && h[0] == 0xFF && h[1] == 0xD8 && h[2] == 0xFF;
        var png = h.Length >= 8 && h[..8].SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });
        var webp = h.Length >= 12 && h[..4].SequenceEqual("RIFF"u8) && h[8..12].SequenceEqual("WEBP"u8);
        return jpeg || png || webp;
    }
}
