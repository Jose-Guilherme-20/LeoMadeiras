
using LeoMadeiras.Application.ViewModels.Common;
using LeoMadeiras.Application.ViewModels.Produtos.Request;
using LeoMadeiras.Application.ViewModels.Produtos.Response;

namespace LeoMadeiras.Application.UseCases.Produtos.ListarProdutos
{
    public interface IListarProdutosUseCase
    {
        Task<PagedResultViewModel<ProdutoResponse>> ExecuteAsync(ProdutoFiltroRequest request, CancellationToken ct = default);
    }
}
