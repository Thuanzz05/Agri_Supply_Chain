using NongDanService.Data;
using NongDanService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register dependencies
builder.Services.AddScoped<INongDanRepository, NongDanRepository>();
builder.Services.AddScoped<INongDanService, NongDanService.Services.NongDanService>();
builder.Services.AddScoped<ITrangTraiRepository, TrangTraiRepository>();
builder.Services.AddScoped<ITrangTraiService, TrangTraiService>();
builder.Services.AddScoped<ISanPhamRepository, SanPhamRepository>();
builder.Services.AddScoped<ISanPhamService, SanPhamService>();
builder.Services.AddScoped<ILoNongSanRepository, LoNongSanRepository>();
builder.Services.AddScoped<ILoNongSanService, LoNongSanService>();
builder.Services.AddScoped<IDonHangRepository, DonHangRepositoryImpl>();
builder.Services.AddScoped<IDonHangService, DonHangService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use CORS
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
