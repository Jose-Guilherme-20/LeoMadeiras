
using LeoMadeiras.Application.Contracts.Repositories;
using LeoMadeiras.Application.ViewModels.Produtos.Response;
using LeoMadeiras.Domain.Exceptions;

namespace LeoMadeiras.Application.UseCases.Produtos.BuscarProduto
{
    public class BuscarProdutoUseCase : IBuscarProdutoUseCase
    {
        private readonly IProdutoRepository _repo;

        public BuscarProdutoUseCase(IProdutoRepository repo) => _repo = repo;

        public async Task<ProdutoResponse> ExecuteAsync(int id, CancellationToken ct = default)
        {
            var produto = await _repo.GetByIdAsync(id, ct)
                ?? throw new NotFoundException($"Produto {id} não encontrado.");

            return new ProdutoResponse
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Descricao = produto.Descricao,
                Preco = produto.Preco,
                QuantidadeEstoque = produto.QuantidadeEstoque,
                CreatedAt = produto.CreatedAt,
                UpdatedAt = produto.UpdatedAt
            };
        }
    }
}
