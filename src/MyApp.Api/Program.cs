using Mediator;
using MyApp.Application.DependencyInjection;
using MyApp.Application.Handlers.Player;
using MyApp.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure();

builder.Services.AddMediator(options =>
{
    options.Assemblies = [typeof(GetPlayerQuery).Assembly];
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
