using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FluentAssertions;
using Fundo.Domain.Entities;
using Fundo.Domain.Enums;
using Fundo.Services.Tests.Integration.Fundo.Applications.WebApi;
using Xunit;

namespace Fundo.Services.Tests.Integration.Fundo.Applications.WebApi.Controllers
{
    public class LoansControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        private readonly HttpClient _client;

        public LoansControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateLoan_WithValidPayload_ShouldReturnCreatedLoan()
        {
            var request = new { amount = 1500.00m, applicantName = "Maria Silva" };

            var response = await _client.PostAsJsonAsync("/api/loans", request);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var loan = await response.Content.ReadFromJsonAsync<Loan>(JsonOptions);
            loan.Should().NotBeNull();
            loan!.Amount.Should().Be(1500.00m);
            loan.CurrentBalance.Should().Be(1500.00m);
            loan.ApplicantName.Should().Be("Maria Silva");
            loan.Status.Should().Be(LoanStatus.Active);
        }

        [Fact]
        public async Task RegisterPayment_WithPartialAmount_ShouldReduceBalanceAndKeepStatusActive()
        {
            var createResponse = await _client.PostAsJsonAsync("/api/loans", new
            {
                amount = 1000.00m,
                applicantName = "João Santos"
            });

            var createdLoan = await createResponse.Content.ReadFromJsonAsync<Loan>(JsonOptions);

            var paymentResponse = await _client.PostAsJsonAsync(
                $"/api/loans/{createdLoan!.Id}/payment",
                new { amount = 250.00m });

            paymentResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var updatedLoan = await paymentResponse.Content.ReadFromJsonAsync<Loan>(JsonOptions);
            updatedLoan.Should().NotBeNull();
            updatedLoan!.CurrentBalance.Should().Be(750.00m);
            updatedLoan.Status.Should().Be(LoanStatus.Active);
        }

        [Fact]
        public async Task RegisterPayment_WithFullAmount_ShouldSetBalanceToZeroAndStatusPaid()
        {
            var createResponse = await _client.PostAsJsonAsync("/api/loans", new
            {
                amount = 800.00m,
                applicantName = "Ana Costa"
            });

            var createdLoan = await createResponse.Content.ReadFromJsonAsync<Loan>(JsonOptions);

            var paymentResponse = await _client.PostAsJsonAsync(
                $"/api/loans/{createdLoan!.Id}/payment",
                new { amount = 800.00m });

            paymentResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var updatedLoan = await paymentResponse.Content.ReadFromJsonAsync<Loan>(JsonOptions);
            updatedLoan.Should().NotBeNull();
            updatedLoan!.CurrentBalance.Should().Be(0m);
            updatedLoan.Status.Should().Be(LoanStatus.Paid);
        }

        [Fact]
        public async Task RegisterPayment_WithAmountGreaterThanBalance_ShouldReturnBadRequest()
        {
            var createResponse = await _client.PostAsJsonAsync("/api/loans", new
            {
                amount = 500.00m,
                applicantName = "Carlos Lima"
            });

            var createdLoan = await createResponse.Content.ReadFromJsonAsync<Loan>(JsonOptions);

            var paymentResponse = await _client.PostAsJsonAsync(
                $"/api/loans/{createdLoan!.Id}/payment",
                new { amount = 600.00m });

            paymentResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var error = await paymentResponse.Content.ReadFromJsonAsync<ErrorResponse>(JsonOptions);
            error.Should().NotBeNull();
            error!.Message.Should().Be("Payment amount cannot exceed the current balance.");
        }

        private sealed class ErrorResponse
        {
            public string Message { get; set; } = string.Empty;
        }
    }
}
