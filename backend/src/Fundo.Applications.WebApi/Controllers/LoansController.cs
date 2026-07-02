using System.Collections.Generic;
using System.Threading.Tasks;
using Fundo.Applications.WebApi.Models.Requests;
using Fundo.Domain.Entities;
using Fundo.Domain.Enums;
using Fundo.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Fundo.Applications.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LoansController : ControllerBase
    {
        private readonly FundoDbContext _context;

        public LoansController(FundoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Loan>>> GetAll()
        {
            var loans = await _context.Loans
                .AsNoTracking()
                .ToListAsync();

            return Ok(loans);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Loan>> GetById(int id)
        {
            var loan = await _context.Loans
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == id);

            if (loan is null)
            {
                return NotFound();
            }

            return Ok(loan);
        }

        [HttpPost]
        public async Task<ActionResult<Loan>> Create([FromBody] CreateLoanRequest request)
        {
            var loan = new Loan
            {
                Amount = request.Amount,
                CurrentBalance = request.Amount,
                ApplicantName = request.ApplicantName,
                Status = LoanStatus.Active
            };

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = loan.Id }, loan);
        }

        [HttpPost("{id:int}/payment")]
        public async Task<ActionResult<Loan>> RegisterPayment(int id, [FromBody] PaymentRequest request)
        {
            Log.Information(
                "Processing payment of {Amount} for loan {LoanId}",
                request.Amount,
                id);

            var loan = await _context.Loans.FirstOrDefaultAsync(l => l.Id == id);

            if (loan is null)
            {
                return NotFound();
            }

            var validationError = ValidatePayment(loan, request.Amount);
            if (!string.IsNullOrEmpty(validationError))
            {
                return BadRequest(new { message = validationError });
            }

            ApplyPayment(loan, request.Amount);
            await _context.SaveChangesAsync();

            return Ok(loan);
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
