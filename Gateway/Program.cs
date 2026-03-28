using Gateway;
using Gateway.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add Controllers
builder.Services.AddControllers();

// Load ocelot.json
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// AppSettings
var appSettingsSection = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<AppSettings>(appSettingsSection);

// JWT Authentication
var appSettings = appSettingsSection.Get<AppSettings>();
var key = Encoding.ASCII.GetBytes(appSettings!.Secret);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("Bearer", x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Ocelot với HttpClientHandler bỏ qua SSL validation cho localhost
builder.Services.AddOcelot()
    .AddDelegatingHandler<BypassSslHandler>(true);

// Handler để bỏ qua SSL certificate validation cho localhost
builder.Services.AddTransient<BypassSslHandler>();

var app = builder.Build();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Map Controllers cho local endpoints (auth, test, etc.)
app.MapControllers();

// JWT Middleware cho login
app.UseMiddleware<JwtMiddleware>();

// Chỉ áp dụng Ocelot cho các route cụ thể, không phải tất cả
app.MapWhen(context => 
    context.Request.Path.StartsWithSegments("/api-nongdan") ||
    context.Request.Path.StartsWithSegments("/api-daily") ||
    context.Request.Path.StartsWithSegments("/api-sieuthi") ||
    context.Request.Path.StartsWithSegments("/api-admin"),
    appBuilder =>
    {
        appBuilder.UseOcelot().Wait();
    });

app.Run();

// Handler class để bypass SSL
public class BypassSslHandler : DelegatingHandler
{
    public BypassSslHandler() : base(new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
    })
    {
    }
}