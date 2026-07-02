using Fundo.Domain.Entities;

namespace Fundo.Application.Abstractions
{
    public interface ILoanRepository
    {
        Task<IReadOnlyList<Loan>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<Loan?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<Loan?> GetTrackedByIdAsync(int id, CancellationToken cancellationToken = default);

        Task AddAsync(Loan loan, CancellationToken cancellationToken = default);

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
