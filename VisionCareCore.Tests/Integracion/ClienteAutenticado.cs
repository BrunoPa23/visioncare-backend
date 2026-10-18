using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace VisionCareCore.Tests.Integracion;

/// <summary>
/// Registra un usuario nuevo, inicia sesion y devuelve un cliente con su JWT en el header Authorization.
/// </summary>
public static class ClienteAutenticado
{
    public static async Task<(HttpClient Cliente, Guid UserId)> CrearAsync(VisionCareApiFactory factory)
    {
        var cliente = factory.CreateClient();
        var email = $"{Guid.NewGuid():N}@test.com";
        const string password = "Clave123!";

        var registro = await cliente.PostAsJsonAsync("/vc/v1/authentication/sign-up",
            new { email, password, name = "Usuario", lastName = "Prueba", visualImpairment = "Leve" });
        registro.EnsureSuccessStatusCode();

        var login = await cliente.PostAsJsonAsync("/vc/v1/authentication/sign-in", new { email, password });
        login.EnsureSuccessStatusCode();

        // El backend entrega el JWT en la cookie AuthToken
        var cookie = login.Headers.GetValues("Set-Cookie").First(c => c.StartsWith("AuthToken="));
        var token = cookie.Split(';')[0]["AuthToken=".Length..];
        cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var yo = await cliente.GetFromJsonAsync<JsonElement>("/vc/v1/auth-user/me");
        return (cliente, yo.GetProperty("id").GetGuid());
    }
}
