
using LeoMadeiras.Domain.Entities;

namespace LeoMadeiras.Application.Contracts.Services
{
    public interface IJwtService
    {
        string GerarToken(Usuario usuario);
        DateTime ObterExpiracao();
    }
}
