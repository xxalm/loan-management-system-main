using Fundo.Domain.Entities;

namespace Fundo.Application.Models
{
    public sealed class LoanPaymentResult
    {
        private LoanPaymentResult(Loan? loan, string? errorMessage, bool isNotFound)
        {
            Loan = loan;
            ErrorMessage = errorMessage;
            IsNotFound = isNotFound;
        }

        public Loan? Loan { get; }

        public string? ErrorMessage { get; }

        public bool IsNotFound { get; }

        public static LoanPaymentResult Success(Loan loan) =>
            new(loan, null, false);

        public static LoanPaymentResult NotFound() =>
            new(null, null, true);

        public static LoanPaymentResult ValidationFailed(string errorMessage) =>
            new(null, errorMessage, false);
    }
}
