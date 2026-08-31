using AgroControl.Application.Abstractions.Data;
using AgroControl.Application.Catalog.Repositories;
using AgroControl.Application.Identity;
using AgroControl.Application.Inventory;
using AgroControl.Infrastructure.Identity;
using AgroControl.Infrastructure.Persistence;
using AgroControl.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AgroControl.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AgroControlDatabase");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'AgroControlDatabase' was not configured.");
        }

        services.AddDbContext<AgroControlDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IUnitOfWork>(provider =>
            provider.GetRequiredService<AgroControlDbContext>());
        services.AddScoped<IAgriculturalInputRepository, AgriculturalInputRepository>();
        services.AddScoped<IInputCategoryRepository, InputCategoryRepository>();
        services.AddScoped<IManufacturerRepository, ManufacturerRepository>();
        services.AddScoped<IMeasurementUnitRepository, MeasurementUnitRepository>();
        services.AddScoped<IStockLotRepository, StockLotRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();

        return services;
    }
}
