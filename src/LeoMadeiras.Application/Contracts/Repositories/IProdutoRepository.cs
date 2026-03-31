
using LeoMadeiras.Application.ViewModels.Common;
using LeoMadeiras.Application.ViewModels.Produtos.Request;
using LeoMadeiras.Application.ViewModels.Produtos.Response;
using LeoMadeiras.Domain.Entities;

namespace LeoMadeiras.Application.Contracts.Repositories
{
    public interface IProdutoRepository : IBaseRepository<Produto>
    {
        Task<PagedResultViewModel<Produto>> GetPagedAsync(ProdutoFiltroRequest filtro, CancellationToken ct = default);
        Task<IEnumerable<MaisVendidoResponse>> GetMaisVendidosAsync(CancellationToken ct = default);
    }
}
