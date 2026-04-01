
using System.ComponentModel.DataAnnotations;

namespace LeoMadeiras.Application.ViewModels.Auth.Request
{
    public class RegistrarUsuarioRequest
    {
        [Required(ErrorMessage = "Nome é obrigatório.")]
        [MaxLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        [MaxLength(200, ErrorMessage = "Email deve ter no máximo 200 caracteres.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória.")]
        [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres.")]
        public string Senha { get; set; } = string.Empty;
    }
}
