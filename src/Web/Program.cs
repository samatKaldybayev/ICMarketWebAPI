using ICMarketWebAPI.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebServices();

builder.Services.AddCors(options =>
    options.AddPolicy("Default", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseExceptionHandler(options => { });

if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors("Default");

app.MapHealthChecks("/health");

app.UseOpenApi(s => s.Path = "/api/specification.json");
app.UseSwaggerUi(s =>
{
    s.Path = "/api";
    s.DocumentPath = "/api/specification.json";
});

app.MapControllers();
app.MapEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapGet("/", () => Results.Redirect("/api"));
}

app.Run();

public partial class Program { }
