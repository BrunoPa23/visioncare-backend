using VisionCareCore.OpenAI.Interfaces.REST.Resources;

namespace VisionCareCore.Vision.Domain.Services
{
    public interface IVisionService
    {
        Task<string> RecognizeImageAsync(IFormFile imageRequest);
        Task<GptResponse> AnalyzeImageAsync(IFormFile imageRequest);
    }
}
