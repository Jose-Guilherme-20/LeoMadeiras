using LeoMadeiras.Application.Contracts.Repositories;
using LeoMadeiras.Application.UseCases.Produtos.ListarProdutos;
using LeoMadeiras.Application.ViewModels.Common;
using LeoMadeiras.Application.ViewModels.Produtos.Request;
using LeoMadeiras.Domain.Entities;
using Moq;

namespace LeoMadeiras.Tests.UseCases.Produtos
{
    public class ListarProdutosUseCaseTests
    {
        private readonly Mock<IProdutoRepository> _repoMock = new();

        private static Produto CriarProduto(int id)
        {
            var p = new Produto("Nome", "Desc", 50m, 10);
            typeof(LeoMadeiras.Domain.Common.BaseEntity)
                .GetProperty("Id")!
                .SetValue(p, id);
            return p;
        }

        [Fact]
        public async Task ExecuteAsync_RetornaPagedResultComItens()
        {
            var produtos = new List<Produto> { CriarProduto(1), CriarProduto(2) };
            var paged = new PagedResultViewModel<Produto>
            {
                Items = produtos,
                Total = 2,
                Page = 1,
                PageSize = 10
            };

            _repoMock
                .Setup(r => r.GetPagedAsync(It.IsAny<ProdutoFiltroRequest>(), default))
                .ReturnsAsync(paged);

            var useCase = new ListarProdutosUseCase(_repoMock.Object);
            var result = await useCase.ExecuteAsync(new ProdutoFiltroRequest());

            Assert.Equal(2, result.Total);
            Assert.Equal(2, result.Items.Count());
        }

        [Fact]
        public async Task ExecuteAsync_SemProdutos_RetornaListaVazia()
        {
            var paged = new PagedResultViewModel<Produto>
            {
                Items = [],
                Total = 0,
                Page = 1,
                PageSize = 10
            };

            _repoMock
                .Setup(r => r.GetPagedAsync(It.IsAny<ProdutoFiltroRequest>(), default))
                .ReturnsAsync(paged);

            var useCase = new ListarProdutosUseCase(_repoMock.Object);
            var result = await useCase.ExecuteAsync(new ProdutoFiltroRequest());

            Assert.Equal(0, result.Total);
            Assert.Empty(result.Items);
        }
    }
}