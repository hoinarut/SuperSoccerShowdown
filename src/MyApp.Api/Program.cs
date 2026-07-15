using Amazon.Lambda.AspNetCoreServer.Hosting;
using FluentValidation;
using FluentValidation.AspNetCore;
using Mediator;
using Microsoft.EntityFrameworkCore;
using MyApp.Api.Validation;
using MyApp.Application.DependencyInjection;
using MyApp.Application.Handlers.Team;
using MyApp.Infrastructure.DependencyInjection;
using MyApp.Infrastructure.Persistence.Ef;
using MyApp.Infrastructure.Persistence.Ef.Seeding;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddMediator(options =>
{
    options.Assemblies = [typeof(GetTeamsHandler).Assembly];
    options.ServiceLifetime = ServiceLifetime.Scoped;
});

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Web", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? ["http://localhost:4200", "http://127.0.0.1:4200"];

        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddOpenApi();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateTeamRequestValidator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<MyAppDbContext>();
    dbContext.Database.Migrate();
    await DatabaseSeeder.SeedAsync(dbContext);
}

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AWS_LAMBDA_FUNCTION_NAME")))
{
    app.UseHttpsRedirection();
}

app.UseCors("Web");
app.MapControllers();

app.Run();

public partial class Program;
