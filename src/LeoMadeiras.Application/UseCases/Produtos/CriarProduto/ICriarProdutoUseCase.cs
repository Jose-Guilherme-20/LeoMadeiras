
using LeoMadeiras.Application.ViewModels.Produtos.Request;
using LeoMadeiras.Application.ViewModels.Produtos.Response;

namespace LeoMadeiras.Application.UseCases.Produtos.CriarProduto
{
    public interface ICriarProdutoUseCase
    {
        Task<ProdutoResponse> ExecuteAsync(CriarProdutoRequest request, CancellationToken ct = default);
    }
}
