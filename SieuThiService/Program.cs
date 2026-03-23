using SieuThiService.Data;
using SieuThiService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register SieuThi dependencies
builder.Services.AddScoped<ISieuThiRepository, SieuThiRepository>();
builder.Services.AddScoped<ISieuThiService, SieuThiBusinessService>();

// Register Kho dependencies for SieuThi
builder.Services.AddScoped<IKhoRepository, KhoRepository>();
builder.Services.AddScoped<IKhoService, KhoService>();

// Register TonKho dependencies for SieuThi
builder.Services.AddScoped<ITonKhoRepository, TonKhoRepository>();
builder.Services.AddScoped<ITonKhoService, TonKhoService>();

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
