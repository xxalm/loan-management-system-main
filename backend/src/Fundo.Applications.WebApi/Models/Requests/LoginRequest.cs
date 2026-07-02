using System.ComponentModel.DataAnnotations;

namespace Fundo.Applications.WebApi.Models.Requests
{
    public class LoginRequest
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
