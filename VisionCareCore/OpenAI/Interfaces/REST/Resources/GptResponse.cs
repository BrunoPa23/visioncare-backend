namespace VisionCareCore.OpenAI.Interfaces.REST.Resources
{
    public class GptResponse
    {
        public string Medicamento { get; init; }
        public string Tipo { get; init; }
        public object Contenido { get; init; }
        public string? ExpirationDate { get; init; }
    }
}
