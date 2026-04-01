using LeoMadeiras.Application.Contracts.Repositories;
using LeoMadeiras.Application.Contracts.Services;
using LeoMadeiras.Application.UseCases.Auth.Login;
using LeoMadeiras.Application.ViewModels.Auth.Request;
using LeoMadeiras.Domain.Entities;
using LeoMadeiras.Domain.Exceptions;
using Moq;

namespace LeoMadeiras.Tests.UseCases.Auth
{
    public class LoginUseCaseTests
    {
        private readonly Mock<IUsuarioRepository> _repoMock = new();
        private readonly Mock<IJwtService> _jwtMock = new();

        private static Usuario CriarUsuario(string senha = "senha123")
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(senha);
            return new Usuario("Leo", "leo@email.com", hash);
        }

        [Fact]
        public async Task ExecuteAsync_CredenciaisValidas_RetornaAuthResponse()
        {
            var usuario = CriarUsuario("senha123");

            _repoMock
                .Setup(r => r.GetByEmailAsync("leo@email.com", default))
                .ReturnsAsync(usuario);

            _jwtMock.Setup(j => j.GerarToken(usuario)).Returns("token_jwt");
            _jwtMock.Setup(j => j.ObterExpiracao()).Returns(DateTime.UtcNow.AddHours(1));

            var useCase = new LoginUseCase(_repoMock.Object, _jwtMock.Object);
            var response = await useCase.ExecuteAsync(new LoginRequest
            {
                Email = "leo@email.com",
                Senha = "senha123"
            });

            Assert.Equal("token_jwt", response.Token);
            Assert.Equal("Leo", response.Nome);
            Assert.Equal("leo@email.com", response.Email);
        }

        [Fact]
        public async Task ExecuteAsync_EmailInexistente_LancaDomainException()
        {
            _repoMock
                .Setup(r => r.GetByEmailAsync("naoexiste@email.com", default))
                .ReturnsAsync((Usuario?)null);

            var useCase = new LoginUseCase(_repoMock.Object, _jwtMock.Object);

            var ex = await Assert.ThrowsAsync<DomainException>(() =>
                useCase.ExecuteAsync(new LoginRequest
                {
                    Email = "naoexiste@email.com",
                    Senha = "qualquer"
                }));

            Assert.Contains("inválidos", ex.Message);
        }

        [Fact]
        public async Task ExecuteAsync_SenhaIncorreta_LancaDomainException()
        {
            var usuario = CriarUsuario("senha123");

            _repoMock
                .Setup(r => r.GetByEmailAsync("leo@email.com", default))
                .ReturnsAsync(usuario);

            var useCase = new LoginUseCase(_repoMock.Object, _jwtMock.Object);

            var ex = await Assert.ThrowsAsync<DomainException>(() =>
                useCase.ExecuteAsync(new LoginRequest
                {
                    Email = "leo@email.com",
                    Senha = "senhaErrada"
                }));

            Assert.Contains("inválidos", ex.Message);
        }
    }
}