using System.Linq;
using Fundo.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fundo.Services.Tests.Integration.Fundo.Applications.WebApi
{
    public class CustomWebApplicationFactory : WebApplicationFactory<global::Fundo.Applications.WebApi.Startup>
    {
        private readonly string _databaseName = $"LoansTestDb_{System.Guid.NewGuid()}";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment(Environments.Development);

            builder.ConfigureServices(services =>
            {
                var descriptors = services
                    .Where(d => d.ServiceType == typeof(DbContextOptions<FundoDbContext>)
                             || d.ServiceType == typeof(FundoDbContext))
                    .ToList();

                foreach (var descriptor in descriptors)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<FundoDbContext>(options =>
                    options.UseInMemoryDatabase(_databaseName));
            });
        }
    }
}
