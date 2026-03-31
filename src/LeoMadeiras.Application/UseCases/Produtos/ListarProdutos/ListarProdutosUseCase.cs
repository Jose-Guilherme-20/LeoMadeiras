
using LeoMadeiras.Application.Contracts.Repositories;
using LeoMadeiras.Application.ViewModels.Common;
using LeoMadeiras.Application.ViewModels.Produtos.Request;
using LeoMadeiras.Application.ViewModels.Produtos.Response;
using LeoMadeiras.Domain.Entities;

namespace LeoMadeiras.Application.UseCases.Produtos.ListarProdutos
{
    public class ListarProdutosUseCase : IListarProdutosUseCase
    {
        private readonly IProdutoRepository _repo;

        public ListarProdutosUseCase(IProdutoRepository repo) => _repo = repo;

        public async Task<PagedResultViewModel<ProdutoResponse>> ExecuteAsync(
            ProdutoFiltroRequest request, CancellationToken ct = default)
        {
            var result = await _repo.GetPagedAsync(request, ct);

            return new PagedResultViewModel<ProdutoResponse>
            {
                Items = result.Items.Select(ToResponse),
                Total = result.Total,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }

        private static ProdutoResponse ToResponse(Produto produto) => new()
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
