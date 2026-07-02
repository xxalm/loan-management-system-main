using Fundo.Application.Models;
using Fundo.Domain.Entities;

namespace Fundo.Application.Services
{
    public interface ILoanService
    {
        Task<IReadOnlyList<Loan>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<Loan?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<Loan> CreateAsync(
            string applicantName,
            decimal amount,
            string contractId,
            string taxId,
            CancellationToken cancellationToken = default);

        Task<LoanPaymentResult> RegisterPaymentAsync(
            int loanId,
            decimal paymentAmount,
            CancellationToken cancellationToken = default);
    }
}
