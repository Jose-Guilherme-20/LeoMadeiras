
using LeoMadeiras.Application.Contracts;
using LeoMadeiras.Application.Contracts.Repositories;
using LeoMadeiras.Application.ViewModels.Produtos.Request;
using LeoMadeiras.Application.ViewModels.Produtos.Response;
using LeoMadeiras.Domain.Exceptions;

namespace LeoMadeiras.Application.UseCases.Produtos.AtualizarProduto
{
    public class AtualizarProdutoUseCase : IAtualizarProdutoUseCase
    {
        private readonly IProdutoRepository _repo;
        private readonly IUnitOfWork _uow;

        public AtualizarProdutoUseCase(IProdutoRepository repo, IUnitOfWork uow)
        {
            _repo = repo;
            _uow = uow;
        }

        public async Task<ProdutoResponse> ExecuteAsync(
            int id, AtualizarProdutoRequest request, CancellationToken ct = default)
        {
            var produto = await _repo.GetByIdAsync(id, ct)
                ?? throw new NotFoundException($"Produto {id} não encontrado.");

            produto.Atualizar(
                request.Nome,
                request.Descricao,
                request.Preco,
                request.QuantidadeEstoque
            );

            await _repo.UpdateAsync(produto, ct);
            await _uow.CommitAsync(ct);

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
