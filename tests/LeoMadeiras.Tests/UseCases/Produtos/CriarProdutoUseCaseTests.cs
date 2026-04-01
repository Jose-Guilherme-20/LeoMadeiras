using LeoMadeiras.Application.Contracts;
using LeoMadeiras.Application.Contracts.Repositories;
using LeoMadeiras.Application.UseCases.Produtos.CriarProduto;
using LeoMadeiras.Application.ViewModels.Produtos.Request;
using LeoMadeiras.Domain.Entities;
using Moq;

namespace LeoMadeiras.Tests.UseCases.Produtos
{
    public class CriarProdutoUseCaseTests
    {
        private readonly Mock<IProdutoRepository> _repoMock = new();
        private readonly Mock<IUnitOfWork> _uowMock = new();

        [Fact]
        public async Task ExecuteAsync_RequestValido_RetornaProdutoResponse()
        {
            var useCase = new CriarProdutoUseCase(_repoMock.Object, _uowMock.Object);
            var request = new CriarProdutoRequest
            {
                Nome = "Mesa",
                Descricao = "Mesa de madeira",
                Preco = 299m,
                QuantidadeEstoque = 5
            };

            var response = await useCase.ExecuteAsync(request);

            Assert.Equal("Mesa", response.Nome);
            Assert.Equal(299m, response.Preco);
            Assert.Equal(5, response.QuantidadeEstoque);
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Produto>(), default), Times.Once);
            _uowMock.Verify(u => u.CommitAsync(default), Times.Once);
        }
    }
}