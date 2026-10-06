using VisionCareCore.OpenAI.Domain.Services;
using VisionCareCore.OpenAI.Infrastructure.ExternalAPIs.OpenAI;
using VisionCareCore.OpenAI.Interfaces.REST.Resources;

namespace VisionCareCore.OpenAI.Application.Internal.CommandServices
{
    public class GptCommandService : IGptService
    {
        private readonly IGptClient _gptClient;
        public GptCommandService(IGptClient gptClient)
        {
            _gptClient = gptClient;
        }
        
        public async Task<GptResponse> ProcessAsync(GptRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            return await _gptClient.SendRequestAsync(request);
        }
    }
}
