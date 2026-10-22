using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using VisionCareCore.Shared.Infraestructure.Persistences.EFC.Configuration;

namespace VisionCareCore.Tests.Integracion;

/// <summary>
/// Levanta la API completa en memoria: SQLite en lugar de SQL Server y configuracion JWT de prueba.
/// No llama a Azure ni a OpenAI; los tests no usan esos endpoints con un token valido.
/// </summary>
public class VisionCareApiFactory : WebApplicationFactory<Program>
{
    // La base SQLite en memoria vive mientras esta conexion siga abierta
    private readonly SqliteConnection _conexion = new("DataSource=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        // UseSetting se aplica antes de que Program lea la configuracion
        builder.UseSetting("ConnectionStrings:DefaultConnection", "no-se-usa-en-tests");
        builder.UseSetting("TokenSettings:Secret", "clave-de-prueba-solo-para-tests-0123456789");
        builder.UseSetting("TokenSettings:Issuer", "visioncare-tests");
        builder.UseSetting("TokenSettings:Audience", "visioncare-tests");

        // Valores falsos para pasar la validacion al arrancar; ningun test llama a Azure ni a OpenAI
        builder.UseSetting("AzureVision:Endpoint", "https://vision.example.test/");
        builder.UseSetting("AzureVision:Key", "clave-falsa");
        builder.UseSetting("OpenAI:ApiKey", "clave-falsa");
        builder.UseSetting("OpenAI:AssistantId", "asistente-falso");

        builder.ConfigureServices(services =>
        {
            // Desde EF Core 9 la configuracion del proveedor tambien se registra aparte
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();

            _conexion.Open();
            services.AddDbContext<AppDbContext>(options => options.UseSqlite(_conexion));
        });
    }

    // Las migraciones son de SQL Server; en SQLite el esquema se crea desde el modelo
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);
        using var scope = host.Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
        return host;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing) _conexion.Dispose();
    }
}
