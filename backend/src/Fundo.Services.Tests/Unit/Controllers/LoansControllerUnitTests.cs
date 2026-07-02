using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Fundo.Applications.WebApi.Controllers;
using Fundo.Applications.WebApi.Models.Requests;
using Fundo.Application.Services;
using Fundo.Domain.Entities;
using Fundo.Domain.Enums;
using Fundo.Infrastructure.Data;
using Fundo.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Fundo.Services.Tests.Unit.Controllers
{
    public class LoansControllerUnitTests
    {
        [Fact]
        public async Task RegisterPayment_WhenLoanDoesNotExist_ShouldReturnNotFound()
        {
            await using var context = new FundoDbContext(CreateOptions());
            var controller = CreateController(context);

            var result = await controller.RegisterPayment(999, new PaymentRequest { Amount = 50m });

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_WithValidRequest_ShouldPersistLoanWithActiveStatus()
        {
            await using var context = new FundoDbContext(CreateOptions());
            var controller = CreateController(context);

            var result = await controller.Create(new CreateLoanRequest
            {
                Amount = 2000m,
                ApplicantName = "Unit Test User",
                ContractId = "CTR-TEST-001",
                TaxId = "123.456.789-01"
            });

            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            var loan = createdResult.Value.Should().BeOfType<Loan>().Subject;

            loan.Amount.Should().Be(2000m);
            loan.CurrentBalance.Should().Be(2000m);
            loan.Status.Should().Be(LoanStatus.Active);
            loan.ContractId.Should().Be("CTR-TEST-001");
            loan.TaxId.Should().Be("123.456.789-01");

            var persistedLoan = await context.Loans.FirstOrDefaultAsync(l => l.Id == loan.Id);
            persistedLoan.Should().NotBeNull();
            persistedLoan!.ApplicantName.Should().Be("Unit Test User");
        }

        [Fact]
        public void Controller_ShouldDependOnILoanService()
        {
            var serviceMock = new Mock<ILoanService>();

            var controller = new LoansController(serviceMock.Object);

            controller.Should().NotBeNull();
            serviceMock.VerifyNoOtherCalls();
        }

        private static LoansController CreateController(FundoDbContext context)
        {
            var repository = new LoanRepository(context);
            var logger = Mock.Of<ILogger<LoanService>>();
            var loanService = new LoanService(repository, logger);
            return new LoansController(loanService);
        }

        private static DbContextOptions<FundoDbContext> CreateOptions()
        {
            return new DbContextOptionsBuilder<FundoDbContext>()
                .UseInMemoryDatabase($"LoansUnitTestDb_{System.Guid.NewGuid()}")
                .Options;
        }
    }
}
