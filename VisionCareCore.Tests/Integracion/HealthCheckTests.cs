using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace VisionCareCore.Tests.Integracion;

public class HealthCheckTests(VisionCareApiFactory factory) : IClassFixture<VisionCareApiFactory>
{
    [Fact]
    public async Task Health_SinToken_RespondeSanoConLaBaseDeDatos()
    {
        var respuesta = await factory.CreateClient().GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        var cuerpo = await respuesta.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Healthy", cuerpo.GetProperty("status").GetString());
        var check = Assert.Single(cuerpo.GetProperty("checks").EnumerateArray());
        Assert.Equal("base-de-datos", check.GetProperty("nombre").GetString());
        Assert.Equal("Healthy", check.GetProperty("status").GetString());
    }
}
