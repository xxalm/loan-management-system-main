using System.ComponentModel.DataAnnotations;

namespace Fundo.Applications.WebApi.Models.Requests
{
    public class PaymentRequest
    {
        [Range(0.01, double.MaxValue, ErrorMessage = "Payment amount must be greater than zero.")]
        public decimal Amount { get; set; }
    }
}
