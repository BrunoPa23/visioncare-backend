namespace VisionCareCore.OpenAI.Interfaces.REST.Resources
{
    public record GptRequest
    {
        public string Tipo { get; init; }
        public string Texto { get; init; }
    }
}
