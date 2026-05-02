using DaiLyService.Data;
using DaiLyService.Services;
using DaiLyService.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add DbContext
builder.Services.AddDbContext<CnwebAgriSupplyChainContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add dependency injection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddScoped<IDaiLyRepository, DaiLyRepository>();
builder.Services.AddScoped<IDaiLyService, DaiLyService.Services.DaiLyService>();
builder.Services.AddScoped<IKhoRepository, KhoRepository>();
builder.Services.AddScoped<IKhoService, KhoService>();
builder.Services.AddScoped<ITonKhoRepository, InventoryRepository>();
builder.Services.AddScoped<ITonKhoService, TonKhoService>();
builder.Services.AddScoped<IDonHangRepository, DonHangRepository>();
builder.Services.AddScoped<IDonHangService, DonHangService>();
builder.Services.AddScoped<IVanChuyenRepository, VanChuyenRepository>();
builder.Services.AddScoped<IVanChuyenService, VanChuyenService>();
builder.Services.AddScoped<IKiemDinhRepository, KiemDinhRepository>();
builder.Services.AddScoped<IKiemDinhService, KiemDinhService>();
builder.Services.AddScoped<IKiemDinhService, KiemDinhService>();
builder.Services.AddScoped<IDashboardRepository>(sp => new DashboardRepository(connectionString));
builder.Services.AddScoped<IDashboardService, DashboardService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
