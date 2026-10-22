using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerUI;
using VisionCareCore.HealthCare.Application.Internal.CommandServices;
using VisionCareCore.HealthCare.Application.Internal.QueryServices;
using VisionCareCore.HealthCare.Domain.Repositories;
using VisionCareCore.HealthCare.Domain.Services;
using VisionCareCore.HealthCare.Infrastructure.Persistence.EFC.Repositories;
using VisionCareCore.OpenAI.Application.Internal.CommandServices;
using VisionCareCore.OpenAI.Domain.Services;
using VisionCareCore.OpenAI.Infrastructure.ExternalAPIs.OpenAI;
using VisionCareCore.Shared.Domain.Repositories;
using VisionCareCore.Shared.Infraestructure.Interfaces.ASP.Configuration;
using VisionCareCore.Shared.Infraestructure.Interfaces.ASP.Configuration.RateLimiting;
using VisionCareCore.Shared.Infraestructure.Persistences.EFC.Configuration;
using VisionCareCore.Shared.Infraestructure.Persistences.EFC.Repositories;
using VisionCareCore.User.Application.Internal.CommandServices;
using VisionCareCore.User.Application.Internal.OutboundServices;
using VisionCareCore.User.Application.Internal.QueryServices;
using VisionCareCore.User.Domain.Repositories;
using VisionCareCore.User.Domain.Services;
using VisionCareCore.User.Infrastructure.Hashing.BCrypt.Services;
using VisionCareCore.User.Infrastructure.Persistence.EFC.Repositories;
using VisionCareCore.User.Infrastructure.Tokens.JWT.Configurations;
using VisionCareCore.User.Infrastructure.Tokens.JWT.Services;
using VisionCareCore.User.Interfaces.ACL;
using VisionCareCore.User.Interfaces.ACL.Services;
using VisionCareCore.Vision.Application.Internal.CommandServices;
using VisionCareCore.Vision.Domain.Services;
using VisionCareCore.Vision.Infrastructure.ExternalAPIs.Azure;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "Bearer";
        options.DefaultChallengeScheme = "Bearer";
    })
    .AddJwtBearer("Bearer", options =>
    {
        var secret = builder.Configuration["TokenSettings:Secret"];
        if (string.IsNullOrEmpty(secret))
        {
            throw new ArgumentNullException(nameof(secret), "JWT Secret is not configured in appsettings.json.");
        }

        var issuer = builder.Configuration["TokenSettings:Issuer"];
        var audience = builder.Configuration["TokenSettings:Audience"];
        if (string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
        {
            throw new InvalidOperationException("JWT Issuer and Audience must be configured in TokenSettings.");
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
        };

        // 🔥 Permitir el uso de cookies para enviar el token
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.ContainsKey("AuthToken"))
                {
                    context.Token = context.Request.Cookies["AuthToken"];
                }
                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,                         
            maxRetryDelay: TimeSpan.FromSeconds(10),  
            errorNumbersToAdd: null                   
        )
    )
    .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
    .EnableDetailedErrors(builder.Environment.IsDevelopment());
});


builder.Services.AddControllers(options =>
{
        options.Conventions.Add(new KebabCaseRouteNamingConvention());
});

// Health check de la API con verificacion de conexion a la base de datos
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("base-de-datos");

// Rate limiting en vision, GPT y sign-in; limites en la seccion RateLimiting
builder.Services.AddVisionCareRateLimiting(builder.Configuration);

// Errores no controlados: se registran en el log y el cliente recibe ProblemDetails sin detalles internos
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Configurar opciones de enrutamiento
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("Cors:OrigenesPermitidos").Get<string[]>() ?? [])  
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();  
    });
});



builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true; // 🔹 Evita acceso JS
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // 🔹 Solo HTTPS en producción
    options.Cookie.SameSite = SameSiteMode.None; // 🔹 Necesario para Swagger y CORS
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "VisionCareCore API",
        Version = "v1",
        Description = "VisionCareCore API",
        TermsOfService = new Uri("http://localhost:5000/swagger/index.html"),
        Contact = new OpenApiContact
        {
            Name = "Bruno",
            Email = "brunomoisespalomino@gmail.com"
        },
        License = new OpenApiLicense
        {
            Name = "Apache 2.0",
            Url = new Uri("https://www.apache.org/licenses/LICENSE-2.0.html")
        }
    });

    // 🔹 Definir el esquema de autenticación JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT con el prefijo 'Bearer '",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    // Microsoft.OpenApi 2: el requisito referencia el esquema por id dentro del documento
    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("TokenSettings"));
builder.Services.AddScoped<IAuthUserRepository, AuthUserRepository>();
builder.Services.AddScoped<IAuthUserRefreshTokenRepository, AuthUserRefreshTokenRepository>();

builder.Services.AddScoped<IAuthUserCommandService, AuthUserCommandService>();
builder.Services.AddScoped<IAuthUserQueryService, AuthUserQueryService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IHashingService, HashingService>();
builder.Services.AddScoped<IIamContextFacade, IamContextFacade>();
// Repositorio de Medicina
builder.Services.AddScoped<IMedicineRepository, MedicineRepository>();

// Command Service de Medicina
builder.Services.AddScoped<IMedicineCommandService, MedicineCommandService>();

// Query Service de Medicina
builder.Services.AddScoped<IMedicineQueryService, MedicineQueryService>();

// Repositorio de MedicineTime
builder.Services.AddScoped<IMedicineTimeRepository, MedicineTimeRepository>();

// Command Service de MedicineTime
builder.Services.AddScoped<IMedicineTimeCommandService, MedicineTimeCommandService>();

// Query Service de MedicineTime
builder.Services.AddScoped<IMedicineTimeQueryService, MedicineTimeQueryService>();

builder.Services.AddOptions<OpenAIOptions>()
    .Bind(builder.Configuration.GetSection(OpenAIOptions.Seccion))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddScoped<IGptClient, GptClient>();
builder.Services.AddScoped<IGptService, GptCommandService>();

builder.Services.AddScoped<IVisionService, VisionCommandService>();
// Sin endpoint o claves validas la API no arranca, en lugar de fallar en la primera peticion
builder.Services.AddOptions<AzureVisionOptions>()
    .Bind(builder.Configuration.GetSection(AzureVisionOptions.Seccion))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddScoped<IVisionClient, VisionClient>();

var app = builder.Build();

// Migraciones al arrancar solo en Development o con Database:AplicarMigraciones=true; en produccion se aplican aparte
if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("Database:AplicarMigraciones"))
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
}

// Configure Middleware Pipeline
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SupportedSubmitMethods(new[] { SubmitMethod.Get, SubmitMethod.Post, SubmitMethod.Delete });
        c.ConfigObject.AdditionalItems["withCredentials"] = true; 
    });}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
// Despues de la autenticacion para limitar por usuario
app.UseRateLimiter();
app.MapControllers();
// Publico para que Docker o un balanceador lo consulten sin token; responde 503 si la base no conecta
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = HealthCheckResponseWriter.EscribirAsync
}).AllowAnonymous();
app.Run();

// Expone Program para los tests de integracion con WebApplicationFactory
public partial class Program { }
