using LeoMadeiras.Application.Contracts.Repositories;
using LeoMadeiras.Application.UseCases.Produtos.BuscarProduto;
using LeoMadeiras.Domain.Entities;
using LeoMadeiras.Domain.Exceptions;
using Moq;

namespace LeoMadeiras.Tests.UseCases.Produtos
{
    public class BuscarProdutoUseCaseTests
    {
        private readonly Mock<IProdutoRepository> _repoMock = new();

        private static Produto CriarProduto(int id = 1)
        {
            var p = new Produto("Nome", "Desc", 50m, 10);
            typeof(LeoMadeiras.Domain.Common.BaseEntity)
                .GetProperty("Id")!
                .SetValue(p, id);
            return p;
        }

        [Fact]
        public async Task ExecuteAsync_IdExistente_RetornaProdutoResponse()
        {
            var produto = CriarProduto(id: 1);
            _repoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(produto);

            var useCase = new BuscarProdutoUseCase(_repoMock.Object);
            var response = await useCase.ExecuteAsync(1);

            Assert.Equal(1, response.Id);
        }

        [Fact]
        public async Task ExecuteAsync_IdInexistente_LancaNotFoundException()
        {
            _repoMock.Setup(r => r.GetByIdAsync(99, default)).ReturnsAsync((Produto?)null);

            var useCase = new BuscarProdutoUseCase(_repoMock.Object);

            await Assert.ThrowsAsync<NotFoundException>(() => useCase.ExecuteAsync(99));
        }
    }
}