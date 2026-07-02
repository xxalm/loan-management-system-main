using System.ComponentModel.DataAnnotations;

namespace Fundo.Applications.WebApi.Models.Requests
{
    public class CreateLoanRequest
    {
        [Required(ErrorMessage = "ApplicantName is required.")]
        [MinLength(1, ErrorMessage = "ApplicantName is required.")]
        public string ApplicantName { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }
    }
}
