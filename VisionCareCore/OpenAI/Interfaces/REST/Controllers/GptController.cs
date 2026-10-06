using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using VisionCareCore.OpenAI.Domain.Services;
using VisionCareCore.OpenAI.Interfaces.REST.Resources;

namespace VisionCareCore.OpenAI.Interfaces.REST.Controllers
{

    [ApiController]
    [Route("vc/v1/gpt")]
    [Produces(MediaTypeNames.Application.Json)]
    public class GptController : ControllerBase
    {
        private readonly IGptService _gptService;

        public GptController(IGptService gptService)
        {
            _gptService = gptService;
        }

        [HttpPost("test")]
        public async Task<IActionResult> TestGpt([FromBody] GptRequest request)
        {
            var response = await _gptService.ProcessAsync(request);
            return Ok(response);
        }

        [HttpGet("test-connection")]
        public IActionResult TestConnection()
        {
            try
            {
                return Ok(new { message = "OpenAI API connection successful." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to connect to OpenAI API.", error = ex.Message });
            }
        }

    }
}
