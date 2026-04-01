using LeoMadeiras.Application.Contracts.Repositories;
using LeoMadeiras.Application.UseCases.Produtos.MaisVendidos;
using LeoMadeiras.Application.ViewModels.Produtos.Response;
using Moq;

namespace LeoMadeiras.Tests.UseCases.Produtos
{
    public class MaisVendidosUseCaseTests
    {
        private readonly Mock<IProdutoRepository> _repoMock = new();

        [Fact]
        public async Task ExecuteAsync_RetornaListaOrdenadaPorQuantidade()
        {
            var dados = new List<MaisVendidoResponse>
            {
                new() { ProdutoId = 1, Nome = "Mesa",    TotalVendido = 10 },
                new() { ProdutoId = 2, Nome = "Cadeira", TotalVendido = 5  }
            };

            _repoMock
                .Setup(r => r.GetMaisVendidosAsync(default))
                .ReturnsAsync(dados);

            var useCase = new MaisVendidosUseCase(_repoMock.Object);
            var result = (await useCase.ExecuteAsync()).ToList();

            Assert.Equal(2, result.Count);
            Assert.Equal("Mesa", result[0].Nome);
            Assert.Equal(10, result[0].TotalVendido);
        }

        [Fact]
        public async Task ExecuteAsync_SemVendas_RetornaListaVazia()
        {
            _repoMock
                .Setup(r => r.GetMaisVendidosAsync(default))
                .ReturnsAsync([]);

            var useCase = new MaisVendidosUseCase(_repoMock.Object);
            var result = await useCase.ExecuteAsync();

            Assert.Empty(result);
        }
    }
}