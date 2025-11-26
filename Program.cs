using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
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

// Inyeccion de dependencia del servicio de correo
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
   AUTENTICACION: JWT (API) + Cookie (MVC)
   ========================= */
builder
    .Services.AddAuthentication("AdminCookie")
    .AddCookie(
        "AdminCookie",
        options =>
        {
            options.Cookie.Name = "AdminCookie";
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.SecurePolicy = CookieSecurePolicy.None; // localhost/dev
            options.ExpireTimeSpan = TimeSpan.FromHours(12);
            options.SlidingExpiration = true;
            options.LoginPath = "/Admin/Login";
            options.AccessDeniedPath = "/Admin/Login";
            options.LogoutPath = "/Admin/Logout";
        }
    )
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
   AUTORIZACION (POLITICAS)
   =========================
   - "Ministerio": SOLO JWT (panel consumira APIs con Bearer desde JS)
   - "Usuario":    SOLO JWT (publico autenticado)
*/
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        "Ministerio",
        policy =>
        {
            policy.RequireRole("Ministerio");
            policy.RequireClaim(ClaimTypes.Role, "Ministerio");
        }
    );

    options.AddPolicy(
        "Usuario",
        policy =>
        {
            policy.RequireRole("Usuario");
            policy.RequireClaim(ClaimTypes.Role, "Usuario");
        }
    );
});

// DI
builder.Services.AddScoped<INoticiaService, NoticiaService>();

var app = builder.Build();

// Swagger solo en dev
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

/* IMPORTANTE: Autenticacion antes de Autorizacion */
app.UseAuthentication();
app.UseAuthorization();

// Rutas MVC
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
