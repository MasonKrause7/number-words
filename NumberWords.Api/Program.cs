using NumberWords.Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Adds the conversion service to the container
builder.Services.AddSingleton<NumberToWordConversionService>();

var app = builder.Build();

app.MapControllers();

app.Run();