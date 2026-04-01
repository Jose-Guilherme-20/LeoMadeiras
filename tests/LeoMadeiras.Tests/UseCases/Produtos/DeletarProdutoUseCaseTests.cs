using LeoMadeiras.Application.Contracts;
using LeoMadeiras.Application.Contracts.Repositories;
using LeoMadeiras.Application.UseCases.Produtos.DeletarProduto;
using LeoMadeiras.Domain.Entities;
using LeoMadeiras.Domain.Exceptions;
using Moq;

namespace LeoMadeiras.Tests.UseCases.Produtos
{
    public class DeletarProdutoUseCaseTests
    {
        private readonly Mock<IProdutoRepository> _repoMock = new();
        private readonly Mock<IUnitOfWork> _uowMock = new();

        private static Produto CriarProduto(int id = 1)
        {
            var p = new Produto("Nome", "Desc", 50m, 10);
            typeof(LeoMadeiras.Domain.Common.BaseEntity)
                .GetProperty("Id")!
                .SetValue(p, id);
            return p;
        }

        [Fact]
        public async Task ExecuteAsync_IdExistente_DeletaECommita()
        {
            var produto = CriarProduto(id: 1);
            _repoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(produto);

            var useCase = new DeletarProdutoUseCase(_repoMock.Object, _uowMock.Object);
            await useCase.ExecuteAsync(1);

            _repoMock.Verify(r => r.DeleteAsync(produto, default), Times.Once);
            _uowMock.Verify(u => u.CommitAsync(default), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_IdInexistente_LancaNotFoundException()
        {
            _repoMock.Setup(r => r.GetByIdAsync(99, default)).ReturnsAsync((Produto?)null);

            var useCase = new DeletarProdutoUseCase(_repoMock.Object, _uowMock.Object);

            await Assert.ThrowsAsync<NotFoundException>(() => useCase.ExecuteAsync(99));
        }
    }
}