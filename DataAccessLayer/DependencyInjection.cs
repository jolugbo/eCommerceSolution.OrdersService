using eCommerce.OrdersService.DataAccessLayer.RepositoryContracts;
using eCommerce.OrdersService.DataAccessLayer.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace eCommerce.OrdersService.DataAccessLayer;

public static class DependencyInjection
{
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionStringTemplate = configuration.GetConnectionString("MongoDB")
            ?? "mongodb://$MONGO_HOST:$MONGO_PORT";
        string mongoHost = configuration["MONGODB_HOST"] ?? "localhost";
        string mongoPort = configuration["MONGODB_PORT"] ?? "27017";
        string databaseName = configuration["MONGODB_DATABASE"] ?? "OrdersDatabase";
        string connectionString = connectionStringTemplate
            .Replace("$MONGO_HOST", mongoHost)
            .Replace("$MONGO_PORT", mongoPort);
        services.AddSingleton<IMongoClient>(new MongoClient(connectionString));
        services.AddScoped<IMongoDatabase>( provider => { 
            IMongoClient client = provider.GetRequiredService<IMongoClient>();
            return client.GetDatabase(databaseName);
        });
        services.AddScoped<IOrdersRepository, OrdersRepository>();
        return services;
    }
}
