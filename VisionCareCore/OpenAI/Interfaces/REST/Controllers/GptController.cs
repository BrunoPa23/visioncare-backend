using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using VisionCareCore.OpenAI.Domain.Services;
using VisionCareCore.OpenAI.Interfaces.REST.Resources;

namespace VisionCareCore.OpenAI.Interfaces.REST.Controllers
{

    [Authorize]
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
            return Ok(new { message = "OpenAI API connection successful." });
        }

    }
}
