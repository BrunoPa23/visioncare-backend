using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace VisionCareCore.Tests.Integracion;

public class ConfiguracionTests
{
    [Theory]
    [InlineData("AzureVision:Endpoint", "")]
    [InlineData("AzureVision:Endpoint", "no-es-una-url")]
    [InlineData("OpenAI:ApiKey", "")]
    public void Arranque_ConfiguracionExternaInvalida_NoArrancaLaApi(string clave, string valor)
    {
        using var factory = new VisionCareApiFactory().WithWebHostBuilder(b => b.UseSetting(clave, valor));

        var error = Record.Exception(() => factory.CreateClient());

        Assert.IsType<OptionsValidationException>(error);
    }
}
