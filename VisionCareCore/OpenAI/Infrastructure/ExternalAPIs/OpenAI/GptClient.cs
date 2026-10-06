using OpenAI;
using OpenAI.Assistants;
using OpenAI.Threads;
using System.Text.Json;
using VisionCareCore.OpenAI.Interfaces.REST.Resources;

namespace VisionCareCore.OpenAI.Infrastructure.ExternalAPIs.OpenAI
{
    public class GptClient : IGptClient
    {
        private readonly OpenAIClient _api;
        private readonly string _assistantId;


        public GptClient()
        {
            _api = new OpenAIClient(Environment.GetEnvironmentVariable("APPSETTING_OpenAI_Key"));
            _assistantId = Environment.GetEnvironmentVariable("APPSETTING_Assistant_Key");

            //Print the keys to the console for debugging purposes (remove in production)
            Console.WriteLine($"OpenAI_Key: {_api}");
            Console.WriteLine($"Assistant_Key: {_assistantId}");
        }

        public async Task<GptResponse> SendRequestAsync(GptRequest request)
        {

            var assistant = await _api.AssistantsEndpoint.RetrieveAssistantAsync(_assistantId);

           
            var jsonInput = JsonSerializer.Serialize(request);
            var thread = await _api.ThreadsEndpoint.CreateThreadAsync();
            var message = await thread.CreateMessageAsync(jsonInput);
            var run = await thread.CreateRunAsync(assistant);

            run = await run.WaitForStatusChangeAsync();
            if (run.Status != RunStatus.Completed)
            {
                throw new Exception($"Run failed with status: {run.Status}");
            }

            var messages = await run.ListMessagesAsync();

            var response = messages.Items.FirstOrDefault();
            if (response == null)
            {
                throw new Exception("No response received from the assistant.");
            }

            var rawJson = response.Content?.FirstOrDefault()?.Text?.ToString();

            var gptResponse = JsonSerializer.Deserialize<GptResponse>(rawJson!, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return new GptResponse
            {
                Medicamento = gptResponse.Medicamento,
                Tipo = gptResponse.Tipo,
                Contenido = gptResponse.Contenido,
                ExpirationDate = gptResponse.ExpirationDate,
            };
        }
    }
}
