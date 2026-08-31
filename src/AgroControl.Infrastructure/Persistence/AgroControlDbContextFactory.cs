using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AgroControl.Infrastructure.Persistence;

public sealed class AgroControlDbContextFactory : IDesignTimeDbContextFactory<AgroControlDbContext>
{
    public AgroControlDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable(
            "ConnectionStrings__AgroControlDatabase");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Set the environment variable 'ConnectionStrings__AgroControlDatabase' before running Entity Framework commands.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<AgroControlDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new AgroControlDbContext(optionsBuilder.Options);
    }
}
