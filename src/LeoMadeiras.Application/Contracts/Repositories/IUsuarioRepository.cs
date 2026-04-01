
using LeoMadeiras.Domain.Entities;

namespace LeoMadeiras.Application.Contracts.Repositories
{
    public interface IUsuarioRepository : IBaseRepository<Usuario>
    {
        Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<bool> ExisteEmailAsync(string email, CancellationToken ct = default);
    }
}
