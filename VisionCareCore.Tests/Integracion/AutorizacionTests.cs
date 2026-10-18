using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace VisionCareCore.Tests.Integracion;

public class AutorizacionTests(VisionCareApiFactory factory) : IClassFixture<VisionCareApiFactory>
{
    [Theory]
    [InlineData("/vc/v1/auth-user/me")]
    [InlineData("/api/medicine/user/00000000-0000-0000-0000-000000000001")]
    [InlineData("/api/medicine-time/00000000-0000-0000-0000-000000000001")]
    [InlineData("/api/medicine-time/by-medicine/00000000-0000-0000-0000-000000000001")]
    public async Task SinToken_EndpointProtegido_Responde401(string ruta)
    {
        var respuesta = await factory.CreateClient().GetAsync(ruta);

        Assert.Equal(HttpStatusCode.Unauthorized, respuesta.StatusCode);
    }

    [Fact]
    public async Task SinToken_EscaneoDeImagen_Responde401()
    {
        var respuesta = await factory.CreateClient().PostAsync("/vc/v1/vision/scan", new MultipartFormDataContent());

        Assert.Equal(HttpStatusCode.Unauthorized, respuesta.StatusCode);
    }
}

public class PropiedadRecursosTests(VisionCareApiFactory factory) : IClassFixture<VisionCareApiFactory>
{
    private static async Task<Guid> CrearMedicamentoAsync(HttpClient cliente, Guid? userIdEnElBody = null)
    {
        var respuesta = await cliente.PostAsJsonAsync("/api/medicine", new { nombre = "Paracetamol", userId = userIdEnElBody });
        respuesta.EnsureSuccessStatusCode();
        var cuerpo = await respuesta.Content.ReadFromJsonAsync<JsonElement>();
        return cuerpo.GetProperty("id").GetGuid();
    }

    private static async Task<int> ContarMedicamentosAsync(HttpClient cliente, Guid userId)
    {
        var lista = await cliente.GetFromJsonAsync<JsonElement>($"/api/medicine/user/{userId}");
        return lista.GetArrayLength();
    }

    [Fact]
    public async Task Medicamentos_DeOtroUsuario_NoSePuedenListarNiBorrar()
    {
        var (clienteA, userA) = await ClienteAutenticado.CrearAsync(factory);
        var (clienteB, _) = await ClienteAutenticado.CrearAsync(factory);
        var medicamentoA = await CrearMedicamentoAsync(clienteA);

        Assert.Equal(HttpStatusCode.Forbidden, (await clienteB.GetAsync($"/api/medicine/user/{userA}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await clienteB.DeleteAsync($"/api/medicine/{medicamentoA}")).StatusCode);

        Assert.Equal(1, await ContarMedicamentosAsync(clienteA, userA));
        Assert.Equal(HttpStatusCode.NoContent, (await clienteA.DeleteAsync($"/api/medicine/{medicamentoA}")).StatusCode);
    }

    [Fact]
    public async Task CrearMedicamento_ConUserIdAjenoEnElBody_QuedaDelUsuarioDelToken()
    {
        var (clienteA, userA) = await ClienteAutenticado.CrearAsync(factory);
        var (clienteB, userB) = await ClienteAutenticado.CrearAsync(factory);

        await CrearMedicamentoAsync(clienteA, userIdEnElBody: userB);

        Assert.Equal(1, await ContarMedicamentosAsync(clienteA, userA));
        Assert.Equal(0, await ContarMedicamentosAsync(clienteB, userB));
    }

    [Fact]
    public async Task Horarios_DeUnMedicamentoAjeno_Responden404()
    {
        var (clienteA, _) = await ClienteAutenticado.CrearAsync(factory);
        var (clienteB, _) = await ClienteAutenticado.CrearAsync(factory);
        var medicamentoA = await CrearMedicamentoAsync(clienteA);

        var respuesta = await clienteB.GetAsync($"/api/medicine-time/by-medicine/{medicamentoA}");

        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);
    }

    [Fact]
    public async Task DatosDeUsuario_SoloElPropioUsuarioLosConsulta()
    {
        var (clienteA, userA) = await ClienteAutenticado.CrearAsync(factory);
        var (clienteB, _) = await ClienteAutenticado.CrearAsync(factory);

        Assert.Equal(HttpStatusCode.OK, (await clienteA.GetAsync($"/vc/v1/auth-user/{userA}")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await clienteB.GetAsync($"/vc/v1/auth-user/{userA}")).StatusCode);
    }
}
