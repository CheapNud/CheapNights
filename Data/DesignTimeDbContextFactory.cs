using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CheapNights.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<CheapNightsDbContext>
{
    public CheapNightsDbContext CreateDbContext(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddUserSecrets<DesignTimeDbContextFactory>()
            .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<CheapNightsDbContext>();

        if (environment == "Development")
        {
            optionsBuilder.UseSqlite(configuration.GetConnectionString("Default") ?? "Data Source=horror.db");
        }
        else
        {
            var pgConnection = configuration.GetConnectionString("Default")
                ?? throw new InvalidOperationException("PostgreSQL connection string 'Default' is required. Add it via: dotnet user-secrets set \"ConnectionStrings:Default\" \"<connection-string>\"");
            optionsBuilder.UseNpgsql(pgConnection);
        }

        return new CheapNightsDbContext(optionsBuilder.Options);
    }
}
