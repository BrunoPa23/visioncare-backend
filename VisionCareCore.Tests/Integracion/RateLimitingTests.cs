using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Options;

namespace VisionCareCore.Tests.Integracion;

/// <summary>
/// Cada test usa su propia API con limites bajos para no depender del orden de ejecucion.
/// Ninguna peticion llega a Azure ni a OpenAI: el escaneo falla en la validacion y test-connection no sale a la red.
/// </summary>
public class RateLimitingTests
{
    private static WebApplicationFactory<Program> CrearApi(string politica, int limite) =>
        new VisionCareApiFactory().WithWebHostBuilder(b =>
        {
            b.UseSetting($"RateLimiting:{politica}:PermitLimit", limite.ToString());
            b.UseSetting($"RateLimiting:{politica}:WindowSeconds", "600");
        });

    private static async Task AssertEs429ConProblemDetailsAsync(HttpResponseMessage respuesta)
    {
        Assert.Equal(HttpStatusCode.TooManyRequests, respuesta.StatusCode);
        Assert.Equal("application/problem+json", respuesta.Content.Headers.ContentType?.MediaType);
        Assert.True(respuesta.Headers.Contains("Retry-After"));
        var cuerpo = await respuesta.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(429, cuerpo.GetProperty("status").GetInt32());
        Assert.Equal("Demasiadas peticiones", cuerpo.GetProperty("title").GetString());
    }

    [Fact]
    public async Task SignIn_SuperandoElLimitePorIp_Responde429()
    {
        using var api = CrearApi("SignIn", 2);
        var cliente = api.CreateClient();
        var credenciales = new { email = "no-existe@test.com", password = "Incorrecta1!" };

        Assert.Equal(HttpStatusCode.Unauthorized, (await cliente.PostAsJsonAsync("/vc/v1/authentication/sign-in", credenciales)).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await cliente.PostAsJsonAsync("/vc/v1/authentication/sign-in", credenciales)).StatusCode);

        await AssertEs429ConProblemDetailsAsync(await cliente.PostAsJsonAsync("/vc/v1/authentication/sign-in", credenciales));
    }

    [Fact]
    public async Task Escaneo_SuperandoElLimitePorUsuario_Responde429()
    {
        using var api = CrearApi("Vision", 1);
        var (cliente, _) = await ClienteAutenticado.CrearAsync(api);

        // Sin imagen la validacion responde 400 antes de llamar a Azure, pero la peticion cuenta
        Assert.Equal(HttpStatusCode.BadRequest, (await cliente.PostAsync("/vc/v1/vision/scan", new MultipartFormDataContent())).StatusCode);

        await AssertEs429ConProblemDetailsAsync(await cliente.PostAsync("/vc/v1/vision/recognize-image", new MultipartFormDataContent()));
    }

    [Fact]
    public async Task Gpt_ElLimiteEsPorUsuario_OtroUsuarioNoQuedaBloqueado()
    {
        using var api = CrearApi("Gpt", 1);
        var (clienteA, _) = await ClienteAutenticado.CrearAsync(api);
        var (clienteB, _) = await ClienteAutenticado.CrearAsync(api);

        Assert.Equal(HttpStatusCode.OK, (await clienteA.GetAsync("/vc/v1/gpt/test-connection")).StatusCode);
        await AssertEs429ConProblemDetailsAsync(await clienteA.GetAsync("/vc/v1/gpt/test-connection"));

        Assert.Equal(HttpStatusCode.OK, (await clienteB.GetAsync("/vc/v1/gpt/test-connection")).StatusCode);
    }

    [Fact]
    public void Arranque_LimiteInvalido_NoArrancaLaApi()
    {
        using var api = CrearApi("SignIn", 0);

        var error = Record.Exception(() => api.CreateClient());

        Assert.IsType<OptionsValidationException>(error);
    }
}
