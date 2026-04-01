
namespace LeoMadeiras.Application.ViewModels.Auth.Response
{
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}
