using Azure;
using Azure.AI.Vision.ImageAnalysis;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.ConstrainedExecution;
using static System.Net.Mime.MediaTypeNames;

namespace VisionCareCore.Vision.Infrastructure.ExternalAPIs.Azure
{
    public class VisionClient : IVisionClient
    {
        private readonly string _endpoint;
        private readonly string _visionKey;
        private String completedText = string.Empty;

        public VisionClient()
        {
            _endpoint = Environment.GetEnvironmentVariable("APPSETTING_Vision_Endpoint");
            _visionKey = Environment.GetEnvironmentVariable("APPSETTING_Vision_Key");
        }

        public async Task<string> RecognizeImageAsync(IFormFile imageRequest)
        {

            ImageAnalysisClient client = new ImageAnalysisClient(
                new Uri(_endpoint),
                new AzureKeyCredential(_visionKey));

            using var stream = imageRequest.OpenReadStream();
            BinaryData imageData = BinaryData.FromStream(stream);


            ImageAnalysisResult result = await client.AnalyzeAsync(
                imageData,
                VisualFeatures.Read,
                new ImageAnalysisOptions { GenderNeutralCaption = true }
            );

            foreach (DetectedTextBlock block in result.Read.Blocks)
                foreach (DetectedTextLine line in block.Lines)
                {
                    completedText += line.Text + " ";

                }
            return completedText.TrimEnd();
        }
    }
}
 