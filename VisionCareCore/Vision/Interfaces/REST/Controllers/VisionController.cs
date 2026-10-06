using Azure;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using VisionCareCore.Vision.Domain.Services;

namespace VisionCareCore.Vision.Interfaces.REST.Controllers
{
    [ApiController]
    [Route("vc/v1/vision")]
    [Produces(MediaTypeNames.Application.Json)]

    public class VisionController : ControllerBase
    {
        private readonly IVisionService _visionService;

        public VisionController(IVisionService visionService)
        {
            _visionService = visionService ?? throw new ArgumentNullException(nameof(visionService));
        }

        [HttpPost("recognize-image")]
        public async Task<IActionResult> RecognizeImage(IFormFile imageRequest)
        {
            if (imageRequest == null || imageRequest.Length == 0)
            {
                return BadRequest(new { message = "Invalid image file." });
            }
            try
            {
                var result = await _visionService.AnalyzeImageAsync(imageRequest);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to recognize image.", error = ex.Message });
            }
        }

        [HttpPost("scan")]
        public async Task<IActionResult> ScanImage(IFormFile imageRequest)
        {
            if (imageRequest == null || imageRequest.Length == 0)
            {
                return BadRequest(new { message = "Invalid image file." });
            }
            try
            {
                var result = await _visionService.RecognizeImageAsync(imageRequest);
                return Ok(new { message = "Image scan successfully.", data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to scan image.", error = ex.Message });
            }
        }
    }
}
