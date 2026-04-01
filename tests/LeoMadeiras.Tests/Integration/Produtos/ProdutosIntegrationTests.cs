using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using LeoMadeiras.Application.ViewModels.Common;
using LeoMadeiras.Application.ViewModels.Produtos.Request;
using LeoMadeiras.Application.ViewModels.Produtos.Response;

namespace LeoMadeiras.Tests.Integration.Produtos
{
    public class ProdutosIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ProdutosIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        private void AdicionarToken()
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", JwtTestHelper.GerarToken());
        }

        private static CriarProdutoRequest NovoProdutoRequest(string nome = "Mesa") => new()
        {
            Nome = nome,
            Descricao = "Descrição teste",
            Preco = 299m,
            QuantidadeEstoque = 10
        };

        [Fact]
        public async Task GET_Produtos_SemFiltro_Retorna200()
        {
            var response = await _client.GetAsync("/api/produtos");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content
                .ReadFromJsonAsync<PagedResultViewModel<ProdutoResponse>>();

            Assert.NotNull(result);
        }

        [Fact]
        public async Task POST_Produto_SemToken_Retorna401()
        {
            var response = await _client.PostAsJsonAsync("/api/produtos", NovoProdutoRequest());

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task POST_Produto_ComToken_Retorna201()
        {
            AdicionarToken();

            var response = await _client.PostAsJsonAsync("/api/produtos", NovoProdutoRequest("Cadeira"));

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<ProdutoResponse>();
            Assert.NotNull(result);
            Assert.Equal("Cadeira", result!.Nome);
            Assert.Equal(299m, result.Preco);
        }

        [Fact]
        public async Task GET_Produto_IdExistente_Retorna200()
        {
            AdicionarToken();

            var criar = await _client.PostAsJsonAsync("/api/produtos", NovoProdutoRequest("Armário"));
            var criado = await criar.Content.ReadFromJsonAsync<ProdutoResponse>();

            var response = await _client.GetAsync($"/api/produtos/{criado!.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<ProdutoResponse>();
            Assert.Equal("Armário", result!.Nome);
        }

        [Fact]
        public async Task GET_Produto_IdInexistente_Retorna404()
        {
            var response = await _client.GetAsync("/api/produtos/99999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PUT_Produto_ComToken_AtualizaERetorna200()
        {
            AdicionarToken();

            var criar = await _client.PostAsJsonAsync("/api/produtos", NovoProdutoRequest("Sofá"));
            var criado = await criar.Content.ReadFromJsonAsync<ProdutoResponse>();

            var atualizar = new AtualizarProdutoRequest
            {
                Nome = "Sofá Atualizado",
                Descricao = "Nova descrição",
                Preco = 499m,
                QuantidadeEstoque = 5
            };

            var response = await _client.PutAsJsonAsync($"/api/produtos/{criado!.Id}", atualizar);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<ProdutoResponse>();
            Assert.Equal("Sofá Atualizado", result!.Nome);
            Assert.Equal(499m, result.Preco);
        }

        [Fact]
        public async Task PUT_Produto_SemToken_Retorna401()
        {
            var response = await _client.PutAsJsonAsync("/api/produtos/1", new AtualizarProdutoRequest());

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DELETE_Produto_ComToken_Retorna204()
        {
            AdicionarToken();

            var criar = await _client.PostAsJsonAsync("/api/produtos", NovoProdutoRequest("Prateleira"));
            var criado = await criar.Content.ReadFromJsonAsync<ProdutoResponse>();

            var response = await _client.DeleteAsync($"/api/produtos/{criado!.Id}");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DELETE_Produto_SemToken_Retorna401()
        {
            var response = await _client.DeleteAsync("/api/produtos/1");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }


        [Fact]
        public async Task GET_MaisVendidos_Retorna200()
        {
            var response = await _client.GetAsync("/api/produtos/mais-vendidos");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}