using FIAP.FCG.Infrastructure.Dados;
using FIAP.FCG.Ioc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ContextoBancoDadosFCG>(options =>
		options.UseSqlServer(builder.Configuration.GetConnectionString("ConexaoPadrao")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigurarInjecaoDeDependencias();

var app = builder.Build();

using var scope = app.Services.CreateScope();
using var contexto = scope.ServiceProvider.GetRequiredService<ContextoBancoDadosFCG>();
contexto.Database.Migrate();

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
