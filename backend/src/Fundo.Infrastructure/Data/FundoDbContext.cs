using Fundo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Infrastructure.Data
{
    public class FundoDbContext : DbContext
    {
        public FundoDbContext(DbContextOptions<FundoDbContext> options)
            : base(options)
        {
        }

        public DbSet<Loan> Loans => Set<Loan>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FundoDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
