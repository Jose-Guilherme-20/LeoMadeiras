
using LeoMadeiras.Domain.Entities;

namespace LeoMadeiras.Application.Contracts.Repositories
{
    public interface IProdutoRepository : IBaseRepository<Produto>
    {
        Task<PagedResult<Produto>> GetPagedAsync(ProdutoFiltroDto filtro, CancellationToken ct = default);
        Task<IEnumerable<MaisVendidoDto>> GetMaisVendidosAsync(CancellationToken ct = default);
    }
}
