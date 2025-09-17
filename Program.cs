using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// URL de desarrollo
//builder.WebHost.UseUrls("http://127.0.0.1:5020");

// MVC + API
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioning(config =>
{
    config.DefaultApiVersion = new ApiVersion(0, 1);
});

// Inyección de dependencia del servicio de correo
builder.Services.AddScoped<Grandes_Amigos.Services.EmailService>();

// Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v0.1",
        new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Version = "v0.1",
            Title = "Grandes Amigos",
            Description = "API para la plataforma Grandes Amigos.",
        }
    );
    options.EnableAnnotations();
    options.DocumentFilter<ReemplazaVersion>();
    options.OperationFilter<QuitaVersion>();
});

// DB
builder.Services.AddDbContext<ContextoDb>(options =>
    options.UseMySql(
        builder.Configuration["ConnectionStrings:DefaultConnection"],
        new MariaDbServerVersion(new Version(10, 4, 21))
    )
);

/* =========================
   AUTENTICACIÓN: SOLO JWT
   ========================= */
builder
    .Services.AddAuthentication(options =>
    {
        // Todo se autentica con JWT (no cookies)
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = builder.Configuration["TokenAuthentication:Issuer"],
                ValidAudience = builder.Configuration["TokenAuthentication:Audience"],
                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                    System.Text.Encoding.UTF8.GetBytes(
                        builder.Configuration["TokenAuthentication:SecretKey"]
                            ?? throw new InvalidOperationException(
                                "TokenAuthentication:SecretKey is not configured."
                            )
                    )
                ),
            };
    });

/* =========================
   AUTORIZACIÓN (POLÍTICAS)
   =========================
   - "Ministerio": SOLO JWT (panel consumirá APIs con Bearer desde JS)
   - "Usuario":    SOLO JWT (público autenticado)
*/
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        "Ministerio",
        policy =>
        {
            policy.RequireRole("Ministerio");
            policy.RequireClaim(ClaimTypes.Role, "Ministerio");
            policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
        }
    );

    options.AddPolicy(
        "Usuario",
        policy =>
        {
            policy.RequireRole("Usuario");
            policy.RequireClaim(ClaimTypes.Role, "Usuario");
            policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
        }
    );
});

// DI
builder.Services.AddScoped<INoticiaService, NoticiaService>();

var app = builder.Build();

// Swagger sólo en dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
        {
            options.RouteTemplate = "api/docs/{documentName}/docs.json";
        })
        .UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint($"/api/docs/v0.1/docs.json", "Grandes Amigos");
        });
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

/* IMPORTANTE: Autenticación antes de Autorización */
app.UseAuthentication();
app.UseAuthorization();

// Rutas MVC
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
