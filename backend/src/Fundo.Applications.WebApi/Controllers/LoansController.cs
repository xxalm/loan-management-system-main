using System.Collections.Generic;
using System.Threading.Tasks;
using Fundo.Applications.WebApi.Models.Requests;
using Fundo.Domain.Entities;
using Fundo.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fundo.Applications.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Route("loans")]
    public class LoansController : ControllerBase
    {
        private readonly ILoanService _loanService;

        public LoansController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Loan>>> GetAll()
        {
            var loans = await _loanService.GetAllAsync();
            return Ok(loans);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Loan>> GetById(int id)
        {
            var loan = await _loanService.GetByIdAsync(id);

            if (loan is null)
            {
                return NotFound();
            }

            return Ok(loan);
        }

        [HttpPost]
        public async Task<ActionResult<Loan>> Create([FromBody] CreateLoanRequest request)
        {
            var loan = await _loanService.CreateAsync(
                request.ApplicantName,
                request.Amount,
                request.ContractId,
                request.TaxId);

            return CreatedAtAction(nameof(GetById), new { id = loan.Id }, loan);
        }

        [HttpPost("{id:int}/payment")]
        public async Task<ActionResult<Loan>> RegisterPayment(int id, [FromBody] PaymentRequest request)
        {
            var result = await _loanService.RegisterPaymentAsync(id, request.Amount);

            if (result.IsNotFound)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                return BadRequest(new { message = result.ErrorMessage });
            }

            return Ok(result.Loan);
        }
    }
}
