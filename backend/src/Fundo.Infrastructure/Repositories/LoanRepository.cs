using Fundo.Application.Abstractions;
using Fundo.Domain.Entities;
using Fundo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Infrastructure.Repositories
{
    public class LoanRepository : ILoanRepository
    {
        private readonly FundoDbContext _context;

        public LoanRepository(FundoDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Loan>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Loans
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Loan?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Loans
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        }

        public async Task<Loan?> GetTrackedByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Loans
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        }

        public async Task AddAsync(Loan loan, CancellationToken cancellationToken = default)
        {
            await _context.Loans.AddAsync(loan, cancellationToken);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
            _context.SaveChangesAsync(cancellationToken);
    }
}
