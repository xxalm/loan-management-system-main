using Fundo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Fundo.Infrastructure
{
    public class FundoDbContextFactory : IDesignTimeDbContextFactory<FundoDbContext>
    {
        public FundoDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Fundo.Applications.WebApi"))
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<FundoDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new FundoDbContext(optionsBuilder.Options);
        }
    }
}
