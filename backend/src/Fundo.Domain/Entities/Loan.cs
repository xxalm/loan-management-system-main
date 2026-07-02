using Fundo.Domain.Enums;

namespace Fundo.Domain.Entities
{
    public class Loan
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }

        public decimal CurrentBalance { get; set; }

        public string ApplicantName { get; set; } = string.Empty;

        public string ContractId { get; set; } = string.Empty;

        public string TaxId { get; set; } = string.Empty;

        public LoanStatus Status { get; set; }
    }
}
