using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using LeoMadeiras.Application.ViewModels.Produtos.Request;
using LeoMadeiras.Application.ViewModels.Produtos.Response;
using LeoMadeiras.Application.ViewModels.Vendas.Request;
using LeoMadeiras.Application.ViewModels.Vendas.Response;

namespace LeoMadeiras.Tests.Integration.Vendas
{
    public class VendasIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public VendasIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", JwtTestHelper.GerarToken());
        }

        private async Task<ProdutoResponse> CriarProdutoAsync(int estoque = 10, decimal preco = 50m)
        {
            var request = new CriarProdutoRequest
            {
                Nome = "Produto Teste",
                Descricao = "Desc",
                Preco = preco,
                QuantidadeEstoque = estoque
            };

            var response = await _client.PostAsJsonAsync("/api/produtos", request);
            return (await response.Content.ReadFromJsonAsync<ProdutoResponse>())!;
        }

        [Fact]
        public async Task POST_Venda_Valida_Retorna201()
        {
            var produto = await CriarProdutoAsync(estoque: 10, preco: 50m);

            var request = new CriarVendaRequest
            {
                Order = Guid.NewGuid(),
                Status = "pago",
                Itens =
                [
                    new VendaItemRequest
                    {
                        ProdutoId     = produto.Id,
                        Quantidade    = 2,
                        ValorUnitario = 50m
                    }
                ]
            };

            var response = await _client.PostAsJsonAsync("/api/vendas", request);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<VendaResponse>();
            Assert.NotNull(result);
            Assert.Equal(100m, result!.Total);
            Assert.Equal(request.Order, result.Order);
        }

        [Fact]
        public async Task POST_Venda_CalculaTotalCorretamente()
        {
            var produto1 = await CriarProdutoAsync(estoque: 10, preco: 25m);
            var produto2 = await CriarProdutoAsync(estoque: 10, preco: 75.5m);

            var request = new CriarVendaRequest
            {
                Order = Guid.NewGuid(),
                Status = "pago",
                Itens =
                [
                    new VendaItemRequest { ProdutoId = produto1.Id, Quantidade = 2,  ValorUnitario = 25m   },
                    new VendaItemRequest { ProdutoId = produto2.Id, Quantidade = 1,  ValorUnitario = 75.5m }
                ]
            };

            var response = await _client.PostAsJsonAsync("/api/vendas", request);
            var result = await response.Content.ReadFromJsonAsync<VendaResponse>();

            Assert.Equal(125.5m, result!.Total);
        }

        [Fact]
        public async Task POST_Venda_EstoqueInsuficiente_Retorna400()
        {
            var produto = await CriarProdutoAsync(estoque: 1);

            var request = new CriarVendaRequest
            {
                Order = Guid.NewGuid(),
                Status = "pago",
                Itens =
                [
                    new VendaItemRequest
                    {
                        ProdutoId     = produto.Id,
                        Quantidade    = 99,
                        ValorUnitario = 50m
                    }
                ]
            };

            var response = await _client.PostAsJsonAsync("/api/vendas", request);

            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task POST_Venda_QuantidadeInvalida_Retorna400(int quantidade)
        {
            var produto = await CriarProdutoAsync();

            var request = new CriarVendaRequest
            {
                Order = Guid.NewGuid(),
                Status = "pago",
                Itens =
                [
                    new VendaItemRequest
                    {
                        ProdutoId     = produto.Id,
                        Quantidade    = quantidade,
                        ValorUnitario = 50m
                    }
                ]
            };

            var response = await _client.PostAsJsonAsync("/api/vendas", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task POST_Venda_OrderDuplicado_Retorna422()
        {
            var produto = await CriarProdutoAsync(estoque: 20);
            var order = Guid.NewGuid();

            var request = new CriarVendaRequest
            {
                Order = order,
                Status = "pago",
                Itens =
                [
                    new VendaItemRequest
                    {
                        ProdutoId     = produto.Id,
                        Quantidade    = 1,
                        ValorUnitario = 50m
                    }
                ]
            };

            await _client.PostAsJsonAsync("/api/vendas", request);

            var response = await _client.PostAsJsonAsync("/api/vendas", request);

            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }

        [Fact]
        public async Task POST_Venda_DebitaEstoqueCorretamente()
        {
            var produto = await CriarProdutoAsync(estoque: 10);

            var request = new CriarVendaRequest
            {
                Order = Guid.NewGuid(),
                Status = "pago",
                Itens =
                [
                    new VendaItemRequest
                    {
                        ProdutoId     = produto.Id,
                        Quantidade    = 3,
                        ValorUnitario = 50m
                    }
                ]
            };

            await _client.PostAsJsonAsync("/api/vendas", request);

            var getProduto = await _client.GetAsync($"/api/produtos/{produto.Id}");
            var produtoAtualizado = await getProduto.Content.ReadFromJsonAsync<ProdutoResponse>();

            Assert.Equal(7, produtoAtualizado!.QuantidadeEstoque);
        }
    }
}