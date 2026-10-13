using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using VisionCareCore.Vision.Domain.Services;
using VisionCareCore.Vision.Interfaces.REST.Validation;

namespace VisionCareCore.Vision.Interfaces.REST.Controllers
{
    [Authorize]
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
        [RequestSizeLimit(ImageFileValidator.MaxRequestBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = ImageFileValidator.MaxRequestBytes)]
        public async Task<IActionResult> RecognizeImage(IFormFile imageRequest)
        {
            var error = await ImageFileValidator.ValidateAsync(imageRequest);
            if (error is not null)
            {
                return BadRequest(new { message = error });
            }
            var result = await _visionService.AnalyzeImageAsync(imageRequest);
            return Ok(result);
        }

        [HttpPost("scan")]
        [RequestSizeLimit(ImageFileValidator.MaxRequestBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = ImageFileValidator.MaxRequestBytes)]
        public async Task<IActionResult> ScanImage(IFormFile imageRequest)
        {
            var error = await ImageFileValidator.ValidateAsync(imageRequest);
            if (error is not null)
            {
                return BadRequest(new { message = error });
            }
            var result = await _visionService.RecognizeImageAsync(imageRequest);
            return Ok(new { message = "Image scan successfully.", data = result });
        }
    }
}
