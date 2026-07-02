using Fundo.Domain.Entities;
using Fundo.Domain.Enums;

namespace Fundo.Infrastructure.Data.Seed
{
    public static class LoanSeedData
    {
        public static Loan[] GetLoans() =>
            new[]
            {
                new Loan
                {
                    Id = 1,
                    Amount = 1500.00m,
                    CurrentBalance = 500.00m,
                    ApplicantName = "Maria Silva",
                    Status = LoanStatus.Active
                },
                new Loan
                {
                    Id = 2,
                    Amount = 3000.00m,
                    CurrentBalance = 3000.00m,
                    ApplicantName = "João Santos",
                    Status = LoanStatus.Active
                },
                new Loan
                {
                    Id = 3,
                    Amount = 800.00m,
                    CurrentBalance = 0.00m,
                    ApplicantName = "Ana Costa",
                    Status = LoanStatus.Paid
                }
            };
    }
}
