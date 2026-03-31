
using LeoMadeiras.Application.Contracts;
using LeoMadeiras.Application.Contracts.Repositories;
using LeoMadeiras.Application.ViewModels.Vendas.Request;
using LeoMadeiras.Application.ViewModels.Vendas.Response;
using LeoMadeiras.Domain.Entities;
using LeoMadeiras.Domain.Exceptions;

namespace LeoMadeiras.Application.UseCases.Vendas.RegistrarVenda
{
    public class RegistrarVendaUseCase : IRegistrarVendaUseCase
    {
        private readonly IProdutoRepository _produtoRepo;
        private readonly IVendaRepository _vendaRepo;
        private readonly IUnitOfWork _uow;

        public RegistrarVendaUseCase(
            IProdutoRepository produtoRepo,
            IVendaRepository vendaRepo,
            IUnitOfWork uow)
        {
            _produtoRepo = produtoRepo;
            _vendaRepo = vendaRepo;
            _uow = uow;
        }

        public async Task<VendaResponse> ExecuteAsync(CriarVendaRequest request, CancellationToken ct = default)
        {
            if (await _vendaRepo.ExisteOrderAsync(request.Order, ct))
                throw new DomainException("Venda com este Order já registrada.");

            var itens = new List<VendaItem>();

            foreach (var itemRequest in request.Itens)
            {
                var produto = await _produtoRepo.GetByIdAsync(itemRequest.ProdutoId, ct)
                    ?? throw new NotFoundException($"Produto {itemRequest.ProdutoId} não encontrado.");

                produto.DebitarEstoque(itemRequest.Quantidade);
                await _produtoRepo.UpdateAsync(produto, ct);

                itens.Add(new VendaItem(produto.Id, itemRequest.Quantidade, itemRequest.ValorUnitario));
            }

            var venda = new Venda(request.Order, request.Status, itens);
            await _vendaRepo.AddAsync(venda, ct);
            await _uow.CommitAsync(ct);

            return ToResponse(venda);
        }

        private static VendaResponse ToResponse(Venda venda) => new()
        {
            Id = venda.Id,
            Order = venda.Order,
            Status = venda.Status,
            Total = venda.Total,
            CreatedAt = venda.CreatedAt,
            Itens = venda.Itens.Select(i => new VendaItemResponse
            {
                ProdutoId = i.ProdutoId,
                NomeProduto = i.Produto!.Nome,
                Quantidade = i.Quantidade,
                ValorUnitario = i.ValorUnitario
            }).ToList()
        };
    }
}
