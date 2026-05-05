using NumberWords.Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Adds the conversion service to the container
builder.Services.AddScoped<NumberWordsConversionService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("FrontendPolicy");

app.MapControllers();

app.Run();