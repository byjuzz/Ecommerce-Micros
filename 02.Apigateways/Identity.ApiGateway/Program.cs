using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;
using Identity.ApiGateway.Extensions;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// 🔧 Configuración Ocelot
builder.Configuration
    .AddJsonFile("Configuration/ocelot.combined.json", optional: false, reloadOnChange: true);

// ✅ Leer clave JWT desde appsettings
var secret = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(secret))
    throw new InvalidOperationException("❌ Jwt:Key no está definido en la configuración.");

var key = Encoding.UTF8.GetBytes(secret);

// ✅ Configurar autenticación JWT
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.RequireHttpsMetadata = false;
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

// ✅ Política CORS personalizada
builder.Services.AddCustomCors(builder.Configuration);

// ✅ API Controllers
builder.Services.AddControllers();

// ✅ Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Ocelot
builder.Services.AddOcelot();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = false;
});

var app = builder.Build();

// ✅ Middleware: CORS y JWT desde cookie
app.UseCors("AllowFrontend");

app.Use(async (context, next) =>
{
    if (context.Request.Cookies.TryGetValue("access_token", out var token))
    {
        context.Request.Headers["Authorization"] = $"Bearer {token}";
    }
    await next();
});

app.UseRouting();

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); // 👈 Esto permite que tus controladores personalizados funcionen

//await app.UseOcelot();

app.Logger.LogInformation("🚀 Identity.ApiGateway iniciado en el entorno: {env}", app.Environment.EnvironmentName);

app.Run();
