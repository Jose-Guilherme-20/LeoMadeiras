using System.Net;
using System.Net.Http.Json;
using LeoMadeiras.Application.ViewModels.Auth.Request;
using LeoMadeiras.Application.ViewModels.Auth.Response;

namespace LeoMadeiras.Tests.Integration.Auth
{
    public class AuthIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public AuthIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        private static RegistrarUsuarioRequest NovoUsuarioRequest(string email = "teste@email.com") => new()
        {
            Nome = "Teste",
            Email = email,
            Senha = "senha123"
        };

        [Fact]
        public async Task POST_Registrar_EmailNovo_Retorna201ComToken()
        {
            var response = await _client.PostAsJsonAsync(
                "/api/auth/registrar", NovoUsuarioRequest($"novo_{Guid.NewGuid()}@email.com"));

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            Assert.NotNull(result);
            Assert.False(string.IsNullOrEmpty(result!.Token));
        }

        [Fact]
        public async Task POST_Registrar_EmailDuplicado_Retorna400()
        {
            var email = $"dup_{Guid.NewGuid()}@email.com";

            await _client.PostAsJsonAsync("/api/auth/registrar", NovoUsuarioRequest(email));
            var response = await _client.PostAsJsonAsync("/api/auth/registrar", NovoUsuarioRequest(email));

            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }

        [Fact]
        public async Task POST_Login_CredenciaisValidas_Retorna200ComToken()
        {
            var email = $"login_{Guid.NewGuid()}@email.com";
            await _client.PostAsJsonAsync("/api/auth/registrar", NovoUsuarioRequest(email));

            var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
            {
                Email = email,
                Senha = "senha123"
            });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            Assert.NotNull(result);
            Assert.False(string.IsNullOrEmpty(result!.Token));
        }

        [Fact]
        public async Task POST_Login_SenhaErrada_Retorna400()
        {
            var email = $"errada_{Guid.NewGuid()}@email.com";
            await _client.PostAsJsonAsync("/api/auth/registrar", NovoUsuarioRequest(email));

            var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
            {
                Email = email,
                Senha = "senhaErrada"
            });

            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }

        [Fact]
        public async Task POST_Login_EmailInexistente_Retorna400()
        {
            var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
            {
                Email = "naoexiste@email.com",
                Senha = "qualquer"
            });

            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }
    }
}