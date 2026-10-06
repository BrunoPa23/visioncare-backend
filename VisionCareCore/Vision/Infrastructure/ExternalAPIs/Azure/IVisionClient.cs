namespace VisionCareCore.Vision.Infrastructure.ExternalAPIs.Azure
{
    public interface IVisionClient
    {
        Task<string> RecognizeImageAsync(IFormFile imageBytes);
    }
}
