
using LeoMadeiras.Domain.Common;

namespace LeoMadeiras.Domain.Entities
{
    public class Usuario : BaseEntity
    {
        public string Nome { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string SenhaHash { get; private set; } = string.Empty;

        protected Usuario() { }

        public Usuario(string nome, string email, string senhaHash)
        {
            Nome = nome;
            Email = email;
            SenhaHash = senhaHash;
        }

        public void AtualizarSenha(string novaSenhaHash)
        {
            SenhaHash = novaSenhaHash;
            SetUpdatedAt();
        }
    }
}
