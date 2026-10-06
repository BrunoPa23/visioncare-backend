using System.Runtime.ConstrainedExecution;
using VisionCareCore.OpenAI.Infrastructure.ExternalAPIs.OpenAI;
using VisionCareCore.OpenAI.Interfaces.REST.Resources;
using VisionCareCore.Vision.Domain.Services;
using VisionCareCore.Vision.Infrastructure.ExternalAPIs.Azure;

namespace VisionCareCore.Vision.Application.Internal.CommandServices
{
    public class VisionCommandService : IVisionService
    {
        private readonly IVisionClient _visionClient;
        private readonly IGptClient _gptClient;

        public VisionCommandService(IVisionClient visionClient, IGptClient gptClient)
        {
            _visionClient = visionClient ?? throw new ArgumentNullException(nameof(visionClient));
            _gptClient = gptClient ?? throw new ArgumentNullException(nameof(gptClient));
        }

        public async Task<string> RecognizeImageAsync(IFormFile imageRequest)
        {
            if (imageRequest == null) throw new ArgumentNullException(nameof(imageRequest));
            return await _visionClient.RecognizeImageAsync(imageRequest);
        }

        public async Task<GptResponse> AnalyzeImageAsync(IFormFile imageRequest)
        {
            if (imageRequest == null) throw new ArgumentNullException(nameof(imageRequest));
            var resultVision = await _visionClient.RecognizeImageAsync(imageRequest);

            GptRequest gptRequest = new GptRequest
            {
                Tipo = "que_es",
                Texto = resultVision
            };

            return await _gptClient.SendRequestAsync(gptRequest);
        }
    }
}
