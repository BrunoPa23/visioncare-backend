using System.Net;
using System.Net.Http.Json;

namespace VisionCareCore.Tests.Integracion;

public class ArranqueApiTests(VisionCareApiFactory factory) : IClassFixture<VisionCareApiFactory>
{
    [Fact]
    public async Task Registro_ConBaseEnMemoria_CreaElUsuario()
    {
        var cliente = factory.CreateClient();

        var respuesta = await cliente.PostAsJsonAsync("/vc/v1/authentication/sign-up", new
        {
            email = "arranque@test.com",
            password = "Clave123!",
            name = "Ana",
            lastName = "Prueba",
            visualImpairment = "Leve"
        });

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
    }
}
