
namespace LeoMadeiras.Application.UseCases.Produtos.DeletarProduto
{
    public interface IDeletarProdutoUseCase
    {
        Task ExecuteAsync(int id, CancellationToken ct = default);
    }
}
