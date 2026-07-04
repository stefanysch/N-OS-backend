using Microsoft.EntityFrameworkCore;
using N_OS.Application.Interfaces;
using N_OS.Application.Services;
using N_OS.Domain.Interfaces;
using N_OS.Infrastructure.Data;
using N_OS.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)
);

builder.Services.AddScoped<IPecaRepository, PecaRepository>();
builder.Services.AddScoped<IServicoRepository, ServicoRepository>();

builder.Services.AddScoped<IPecaService, PecaService>();
builder.Services.AddScoped<IServicoService, ServicoService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("frontend");

app.MapGet("/", () => "API rodando 🚀");

app.UseAuthorization();

app.MapControllers();

app.Run();