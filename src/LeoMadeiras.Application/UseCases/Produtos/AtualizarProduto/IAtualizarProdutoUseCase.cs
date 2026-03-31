
using LeoMadeiras.Application.ViewModels.Produtos.Request;
using LeoMadeiras.Application.ViewModels.Produtos.Response;

namespace LeoMadeiras.Application.UseCases.Produtos.AtualizarProduto
{
    public interface IAtualizarProdutoUseCase
    {
        Task<ProdutoResponse> ExecuteAsync(int id, AtualizarProdutoRequest request, CancellationToken ct = default);
    }
}
