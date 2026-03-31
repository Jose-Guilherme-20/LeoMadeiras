
using LeoMadeiras.Application.ViewModels.Produtos.Response;

namespace LeoMadeiras.Application.UseCases.Produtos.BuscarProduto
{
    public interface IBuscarProdutoUseCase
    {
        Task<ProdutoResponse> ExecuteAsync(int id, CancellationToken ct = default);
    }
}
