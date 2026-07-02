using Fundo.Domain.Entities;
using Fundo.Domain.Enums;
using Fundo.Infrastructure.Data.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fundo.Infrastructure.Data.Configurations
{
    public class LoanConfiguration : IEntityTypeConfiguration<Loan>
    {
        public void Configure(EntityTypeBuilder<Loan> builder)
        {
            builder.ToTable("Loans");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.Id)
                .ValueGeneratedOnAdd();

            builder.Property(l => l.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(l => l.CurrentBalance)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(l => l.ApplicantName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(l => l.Status)
                .HasConversion(
                    status => status.ToString().ToLowerInvariant(),
                    value => ParseLoanStatus(value))
                .HasMaxLength(20)
                .IsRequired();

            builder.HasData(LoanSeedData.GetLoans());
        }

        private static LoanStatus ParseLoanStatus(string value) =>
            Enum.Parse<LoanStatus>(value, true);
    }
}
