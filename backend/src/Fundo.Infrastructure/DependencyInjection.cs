using Fundo.Application.Abstractions;
using Fundo.Infrastructure.Data;
using Fundo.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Fundo.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            string connectionString)
        {
            services.AddDbContext<FundoDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<ILoanRepository, LoanRepository>();

            return services;
        }
    }
}
