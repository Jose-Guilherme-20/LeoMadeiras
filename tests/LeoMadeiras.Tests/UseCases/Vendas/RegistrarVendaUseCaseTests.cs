using LeoMadeiras.Application.Contracts;
using LeoMadeiras.Application.Contracts.Repositories;
using LeoMadeiras.Application.UseCases.Vendas.RegistrarVenda;
using LeoMadeiras.Application.ViewModels.Vendas.Request;
using LeoMadeiras.Domain.Entities;
using LeoMadeiras.Domain.Exceptions;
using Moq;

namespace LeoMadeiras.Tests.UseCases.Vendas
{
    public class RegistrarVendaUseCaseTests
    {
        private readonly Mock<IProdutoRepository> _produtoRepoMock;
        private readonly Mock<IVendaRepository> _vendaRepoMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly RegistrarVendaUseCase _useCase;

        public RegistrarVendaUseCaseTests()
        {
            _produtoRepoMock = new Mock<IProdutoRepository>();
            _vendaRepoMock = new Mock<IVendaRepository>();
            _uowMock = new Mock<IUnitOfWork>();

            _useCase = new RegistrarVendaUseCase(
                _produtoRepoMock.Object,
                _vendaRepoMock.Object,
                _uowMock.Object
            );
        }


        private static Produto CriarProduto(int estoque = 10, decimal preco = 50m)
            => new("Produto Teste", "Descrição", preco, estoque);

        private static CriarVendaRequest CriarRequest(
            int produtoId,
            int quantidade,
            decimal valorUnitario,
            Guid? order = null)
            => new()
            {
                Order = order ?? Guid.NewGuid(),
                Status = "pago",
                Itens =
                [
                    new VendaItemRequest
                    {
                        ProdutoId      = produtoId,
                        Quantidade     = quantidade,
                        ValorUnitario  = valorUnitario
                    }
                ]
            };


        [Fact]
        public async Task ExecuteAsync_VendaValida_RetornaVendaResponse()
        {
            var produto = CriarProduto(estoque: 10, preco: 50m);
            var request = CriarRequest(produtoId: 1, quantidade: 2, valorUnitario: 50m);

            _vendaRepoMock
                .Setup(r => r.ExisteOrderAsync(request.Order, default))
                .ReturnsAsync(false);

            _produtoRepoMock
                .Setup(r => r.GetByIdAsync(1, default))
                .ReturnsAsync(produto);

            var response = await _useCase.ExecuteAsync(request);

            Assert.NotNull(response);
            Assert.Equal(request.Order, response.Order);
            Assert.Equal("pago", response.Status);

            _vendaRepoMock.Verify(r => r.AddAsync(It.IsAny<Venda>(), default), Times.Once);
            _uowMock.Verify(u => u.CommitAsync(default), Times.Once);
        }


        [Fact]
        public async Task ExecuteAsync_VendaValida_CalculaTotalCorretamente()
        {
            var produto1 = CriarProduto(estoque: 10, preco: 25m);
            var produto2 = CriarProduto(estoque: 10, preco: 75.5m);

            var order = Guid.NewGuid();
            var request = new CriarVendaRequest
            {
                Order = order,
                Status = "pago",
                Itens =
                [
                    new VendaItemRequest { ProdutoId = 1, Quantidade = 2,  ValorUnitario = 25m   },
                    new VendaItemRequest { ProdutoId = 2, Quantidade = 1,  ValorUnitario = 75.5m }
                ]
            };

            _vendaRepoMock
                .Setup(r => r.ExisteOrderAsync(order, default))
                .ReturnsAsync(false);

            _produtoRepoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(produto1);
            _produtoRepoMock.Setup(r => r.GetByIdAsync(2, default)).ReturnsAsync(produto2);

            var response = await _useCase.ExecuteAsync(request);

            Assert.Equal(125.5m, response.Total);
        }


        [Fact]
        public async Task ExecuteAsync_EstoqueInsuficiente_LancaDomainException()
        {
            var produto = CriarProduto(estoque: 1);
            var request = CriarRequest(produtoId: 1, quantidade: 5, valorUnitario: 50m);

            _vendaRepoMock
                .Setup(r => r.ExisteOrderAsync(request.Order, default))
                .ReturnsAsync(false);

            _produtoRepoMock
                .Setup(r => r.GetByIdAsync(1, default))
                .ReturnsAsync(produto);

            var ex = await Assert.ThrowsAsync<DomainException>(
                () => _useCase.ExecuteAsync(request));

            Assert.Contains("Estoque insuficiente", ex.Message);
        }


        [Fact]
        public async Task ExecuteAsync_VendaValida_DebitaEstoqueCorretamente()
        {
            var produto = CriarProduto(estoque: 10);
            var request = CriarRequest(produtoId: 1, quantidade: 3, valorUnitario: 50m);

            _vendaRepoMock
                .Setup(r => r.ExisteOrderAsync(request.Order, default))
                .ReturnsAsync(false);

            _produtoRepoMock
                .Setup(r => r.GetByIdAsync(1, default))
                .ReturnsAsync(produto);

            await _useCase.ExecuteAsync(request);

            Assert.Equal(7, produto.QuantidadeEstoque);
            _produtoRepoMock.Verify(r => r.UpdateAsync(produto, default), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-99)]
        public async Task ExecuteAsync_QuantidadeInvalida_LancaDomainException(int quantidade)
        {
            var produto = CriarProduto(estoque: 10);
            var request = CriarRequest(produtoId: 1, quantidade: quantidade, valorUnitario: 50m);

            _vendaRepoMock
                .Setup(r => r.ExisteOrderAsync(request.Order, default))
                .ReturnsAsync(false);

            _produtoRepoMock
                .Setup(r => r.GetByIdAsync(1, default))
                .ReturnsAsync(produto);

            var ex = await Assert.ThrowsAsync<DomainException>(
                () => _useCase.ExecuteAsync(request));

            Assert.Contains("maior que zero", ex.Message);
        }


        [Fact]
        public async Task ExecuteAsync_OrderDuplicado_LancaDomainException()
        {
            var order = Guid.NewGuid();
            var request = CriarRequest(produtoId: 1, quantidade: 1, valorUnitario: 50m, order: order);

            _vendaRepoMock
                .Setup(r => r.ExisteOrderAsync(order, default))
                .ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<DomainException>(
                () => _useCase.ExecuteAsync(request));

            Assert.Contains("já registrada", ex.Message);
        }


        [Fact]
        public async Task ExecuteAsync_ProdutoNaoEncontrado_LancaNotFoundException()
        {
            var request = CriarRequest(produtoId: 999, quantidade: 1, valorUnitario: 50m);

            _vendaRepoMock
                .Setup(r => r.ExisteOrderAsync(request.Order, default))
                .ReturnsAsync(false);

            _produtoRepoMock
                .Setup(r => r.GetByIdAsync(999, default))
                .ReturnsAsync((Produto?)null);

            await Assert.ThrowsAsync<NotFoundException>(
                () => _useCase.ExecuteAsync(request));
        }
    }
}