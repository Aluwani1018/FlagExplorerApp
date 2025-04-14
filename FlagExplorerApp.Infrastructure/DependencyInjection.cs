
using FlagExplorerApp.Domain.Common.Interfaces;
using FlagExplorerApp.Domain.Repositories;
using FlagExplorerApp.Infrastructure.Persistance;
using FlagExplorerApp.Infrastructure.Persistance.Documents;
using FlagExplorerApp.Infrastructure.Respositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlagExplorerApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        Console.WriteLine($"AccountEndpoint: {configuration["CosmosDb:AccountEndpoint"]}");
        Console.WriteLine($"AccountKey: {configuration["CosmosDb:AccountKey"]}");
        Console.WriteLine($"DatabaseName: {configuration["CosmosDb:DatabaseName"]}");

        services.AddCosmosRepository(options =>
        {
            options.CosmosConnectionString = $"AccountEndpoint={configuration["CosmosDb:AccountEndpoint"]};AccountKey={configuration["CosmosDb:AccountKey"]};";
            options.DatabaseId = configuration["CosmosDb:DatabaseName"];
            options.ContainerPerItemType = true;

            options.ContainerBuilder
                .Configure<CountryDocument>(c => c
                    .WithContainer("CountryDocument")
                    .WithPartitionKey("/id"))
                .Configure<CountryDetailDocument>(c => c
                    .WithContainer("CountryDetailDocument")
                    .WithPartitionKey("/id"));
        });

        services.AddScoped<ICountryRepository, CountryCosmosDBRepository>();
        services.AddScoped<ICountryDetailRepository, CountryDetailCosmosDBRepository>();
        services.AddScoped<CosmosDBUnitOfWork>();
        services.AddScoped<ICosmosDBUnitOfWork>(provider => provider.GetRequiredService<CosmosDBUnitOfWork>());

        return services;
    }
}
