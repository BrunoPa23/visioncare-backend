using VisionCareCore.OpenAI.Interfaces.REST.Resources;

namespace VisionCareCore.OpenAI.Infrastructure.ExternalAPIs.OpenAI
{
    public interface IGptClient
    {
        Task<GptResponse> SendRequestAsync(GptRequest request);
    }
}
