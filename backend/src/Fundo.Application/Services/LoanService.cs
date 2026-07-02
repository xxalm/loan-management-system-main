using Fundo.Application.Abstractions;
using Fundo.Application.Models;
using Fundo.Domain.Entities;
using Fundo.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Fundo.Application.Services
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly ILogger<LoanService> _logger;

        public LoanService(ILoanRepository loanRepository, ILogger<LoanService> logger)
        {
            _loanRepository = loanRepository;
            _logger = logger;
        }

        public Task<IReadOnlyList<Loan>> GetAllAsync(CancellationToken cancellationToken = default) =>
            _loanRepository.GetAllAsync(cancellationToken);

        public Task<Loan?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
            _loanRepository.GetByIdAsync(id, cancellationToken);

        public async Task<Loan> CreateAsync(
            string applicantName,
            decimal amount,
            string contractId,
            string taxId,
            CancellationToken cancellationToken = default)
        {
            var loan = new Loan
            {
                Amount = amount,
                CurrentBalance = amount,
                ApplicantName = applicantName,
                ContractId = contractId,
                TaxId = taxId,
                Status = LoanStatus.Active
            };

            await _loanRepository.AddAsync(loan, cancellationToken);
            await _loanRepository.SaveChangesAsync(cancellationToken);

            return loan;
        }

        public async Task<LoanPaymentResult> RegisterPaymentAsync(
            int loanId,
            decimal paymentAmount,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Processing payment of {Amount} for loan {LoanId}",
                paymentAmount,
                loanId);

            var loan = await _loanRepository.GetTrackedByIdAsync(loanId, cancellationToken);

            if (loan is null)
            {
                return LoanPaymentResult.NotFound();
            }

            var validationError = ValidatePayment(loan, paymentAmount);
            if (!string.IsNullOrEmpty(validationError))
            {
                return LoanPaymentResult.ValidationFailed(validationError);
            }

            ApplyPayment(loan, paymentAmount);
            await _loanRepository.SaveChangesAsync(cancellationToken);

            return LoanPaymentResult.Success(loan);
        }

        private static string ValidatePayment(Loan loan, decimal paymentAmount)
        {
            if (paymentAmount <= 0)
            {
                return "Payment amount must be greater than zero.";
            }

            if (paymentAmount > loan.CurrentBalance)
            {
                return "Payment amount cannot exceed the current balance.";
            }

            return string.Empty;
        }

        private static void ApplyPayment(Loan loan, decimal paymentAmount)
        {
            loan.CurrentBalance -= paymentAmount;

            if (loan.CurrentBalance == 0)
            {
                loan.Status = LoanStatus.Paid;
            }
        }
    }
}
