using NumberWords.Api.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();
builder.Services.AddControllers();
// Adds the conversion service to the container - scoped means a new instance per HTTP request.
builder.Services.AddSingleton<NumberToWordConversionService>();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();