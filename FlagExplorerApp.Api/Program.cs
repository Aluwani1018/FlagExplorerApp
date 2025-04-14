using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using FlagExplorerApp.Application.Countries.GetCountries; // Ensure this namespace is included
using FlagExplorerApp.Application;
using FlagExplorerApp.Infrastructure;
using FlagExplorerApp.Domain.Entities;
using FlagExplorerApp.Application.Country;
using FlagExplorerApp.Application.CountryDetail;
using FlagExplorerApp.Api.Filters; // Add this namespace if the AddApplication extension method is defined here

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Country API",
        Version = "1.0.0"
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    c.OperationFilter<SwaggerPathOperationFilter>();
});



// Register MediatR and handlers
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(GetCountriesQueryHandler).Assembly);
});



// Ensure the AddApplication extension method is available
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

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
