
using LeoMadeiras.Application.Contracts;
using LeoMadeiras.Application.Contracts.Repositories;
using LeoMadeiras.Application.ViewModels.Produtos.Request;
using LeoMadeiras.Application.ViewModels.Produtos.Response;
using LeoMadeiras.Domain.Entities;

namespace LeoMadeiras.Application.UseCases.Produtos.CriarProduto
{
    public class CriarProdutoUseCase : ICriarProdutoUseCase
    {
        private readonly IProdutoRepository _repo;
        private readonly IUnitOfWork _uow;

        public CriarProdutoUseCase(IProdutoRepository repo, IUnitOfWork uow)
        {
            _repo = repo;
            _uow = uow;
        }

        public async Task<ProdutoResponse> ExecuteAsync(CriarProdutoRequest request, CancellationToken ct = default)
        {
            var produto = new Produto(
                request.Nome,
                request.Descricao,
                request.Preco,
                request.QuantidadeEstoque
            );

            await _repo.AddAsync(produto, ct);
            await _uow.CommitAsync(ct);

            return ToResponse(produto);
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
