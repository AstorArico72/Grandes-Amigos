using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://127.0.0.1:5020");

// builder.Services.AddMvc(); // No es necesario si usas AddControllersWithViews
// builder.Services.AddControllers(); // No es necesario si usas AddControllersWithViews
builder.Services.AddControllersWithViews(); // <-- Agrega esta línea para habilitar controladores y vistas Razor
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioning(config =>
{
    config.DefaultApiVersion = new ApiVersion(0, 1);
});
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v0.1",
        new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Version = "v0.1",
            Title = "Grandes Amigos",
            Description =
                "API para la plataforma Grandes Amigos, una plataforma web centralizada y fácil de usar que conecta a los adultos mayores con las diversas actividades y programas ofrecidos por los distintos ministerios, fomentando la participación, la interacción social y el bienestar.",
            //Pendiente: Completar documentación OpenAPI
        }
    );
    options.EnableAnnotations();
    options.DocumentFilter<ReemplazaVersion>();
    options.OperationFilter<QuitaVersion>();
});
builder.Services.AddDbContext<ContextoDb>(options =>
    options.UseMySql(
        builder.Configuration["ConnectionStrings:DefaultConnection"],
        new MariaDbServerVersion(new Version(10, 4, 21))
    )
);

builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
    options.AddPolicy("Usuario", policy => {
        //Pendiente: Acordar un nombre para ésta política.
        policy.RequireRole("Usuario");
        policy.RequireClaim(ClaimTypes.Role, "Usuario");
        policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
    });
});

// Configuración de servicios para la inyección de dependencias
builder.Services.AddScoped<INoticiaService, NoticiaService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //Pendiente: Manejo de errores
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
