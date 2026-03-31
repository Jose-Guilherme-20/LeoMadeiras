
using LeoMadeiras.Application.Contracts;
using LeoMadeiras.Application.Contracts.Repositories;
using LeoMadeiras.Domain.Exceptions;

namespace LeoMadeiras.Application.UseCases.Produtos.DeletarProduto
{
    public class DeletarProdutoUseCase : IDeletarProdutoUseCase
    {
        private readonly IProdutoRepository _repo;
        private readonly IUnitOfWork _uow;

        public DeletarProdutoUseCase(IProdutoRepository repo, IUnitOfWork uow)
        {
            _repo = repo;
            _uow = uow;
        }

        public async Task ExecuteAsync(int id, CancellationToken ct = default)
        {
            var produto = await _repo.GetByIdAsync(id, ct)
                ?? throw new NotFoundException($"Produto {id} não encontrado.");

            await _repo.DeleteAsync(produto, ct);
            await _uow.CommitAsync(ct);
        }
    }
}
