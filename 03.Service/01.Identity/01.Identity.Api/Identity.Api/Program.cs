using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Identity.Domain;
using Identity.Persistence.Database;
using Identity.Service.EventHandlers.Extensions;
using Identity.Service.Queries.Extensions;
using Identity.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
var builder = WebApplication.CreateBuilder(args);

#region 🗄️ Database (EF Core)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("EcommerceContext"),
        sqlOptions => sqlOptions.MigrationsHistoryTable("_EfMigrationHistory", "Identity")
    ).ConfigureWarnings(warnings =>
        warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)
    )
);
#endregion

#region 🔐 Identity y reglas de contraseña
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;
    options.Password.RequireNonAlphanumeric = false;
});
#endregion

#region 🔐 JWT Authentication (con HttpOnly Cookie)
var secretValue = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(secretValue))
    throw new InvalidOperationException("La clave JWT (Jwt:Key) no está definida en appsettings.json.");
var secretKey = Encoding.UTF8.GetBytes(secretValue);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(secretKey)
        };

        // 🔥 Leer token desde HttpOnly cookie (access_token)
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["access_token"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            }
        };
    });
#endregion
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = false;
});
#region ⚙️ Servicios del dominio
builder.Services.RegisterEventHandlers();
builder.Services.RegisterQueryHandlers();
#endregion

#region 🌐 CORS para frontend (React)
builder.Services.AddCustomCors(builder.Configuration); // 👈 Reemplaza tu AddCors




#endregion

#region 🧭 Middlewares + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity.Api v1");
    });
}

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
        app.Logger.LogInformation("✅ Migraciones aplicadas correctamente.");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "❌ Error al aplicar migraciones automáticamente.");
    }
}

app.Run();
