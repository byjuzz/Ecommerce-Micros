using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;
using Identity.ApiGateway.Extensions;
using Microsoft.AspNetCore.Mvc; // si está en otra carpeta

var builder = WebApplication.CreateBuilder(args);

// 🔧 Configuración Ocelot
builder.Configuration
    .AddJsonFile("Configuration/ocelot.combined.json", optional: false, reloadOnChange: true);

// ✅ Leer clave JWT desde appsettings
var secret = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(secret))
    throw new InvalidOperationException("❌ Jwt:Key no está definido en la configuración.");

var key = Encoding.UTF8.GetBytes(secret); // Recomendado UTF8

// ✅ Configurar autenticación JWT
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.RequireHttpsMetadata = false; // ✅ mejor mantenerlo en true para seguridad
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

// ✅ Política CORS para permitir cookies cross-origin
builder.Services.AddCustomCors(builder.Configuration); // 👈 Reemplaza tu AddCors

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = false;
});

builder.Services.AddOcelot();

var app = builder.Build();

app.UseCors("AllowFrontend");

// 🧠 Middleware: Copiar cookie "access_token" al header Authorization
app.Use(async (context, next) =>
{
    if (context.Request.Cookies.TryGetValue("access_token", out var token))
    {
        context.Request.Headers["Authorization"] = $"Bearer {token}";
    }
    await next();
});

// ✅ HTTPS redirection antes que Ocelot
//app.UseHttpsRedirection();

// ✅ Autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

app.Logger.LogInformation("🚀 Identity.ApiGateway iniciado en el entorno: {env}", app.Environment.EnvironmentName);

// ✅ Ocelot como último
await app.UseOcelot();
app.Run();
