using VisionCareCore.OpenAI.Interfaces.REST.Resources;

namespace VisionCareCore.OpenAI.Domain.Services
{
    public interface IGptService
    {
        Task<GptResponse> ProcessAsync(GptRequest request);
    }
}
