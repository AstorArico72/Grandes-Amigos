using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls ("http://127.0.0.1:5020");
builder.Services.AddMvc ();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ContextoDb> (
    options => options.UseMySql (
        builder.Configuration ["ConnectionStrings:DefaultConnection"], new MariaDbServerVersion (new Version (10, 4, 21))
    )
);

builder.Services.AddAuthentication (JwtBearerDefaults.AuthenticationScheme).AddCookie (options => {
    options.LoginPath = "/Usuario/Ingresar";
}).AddJwtBearer (options => {
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration ["TokenAuthentication:Issuer"],
            ValidAudience = builder.Configuration ["TokenAuthentication:Audience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey (System.Text.Encoding.ASCII.GetBytes (builder.Configuration ["TokenAuthentication:SecretKey"]))
        };
    }
);

builder.Services.AddAuthorization (options => {
    options.AddPolicy ("Ministerio", policy => {
        policy.RequireRole ("Ministerio");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    //Pendiente: Manejo de errores
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
} else {
    app.UseSwagger ();
    app.UseSwaggerUI ();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
