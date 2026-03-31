
using LeoMadeiras.Domain.Entities;

namespace LeoMadeiras.Application.Contracts.Repositories
{
    public interface IVendaRepository : IBaseRepository<Venda>
    {
        Task<bool> ExisteOrderAsync(Guid order, CancellationToken ct = default);
    }
}
