using LeoMadeiras.Application.Contracts;
using LeoMadeiras.Application.Contracts.Repositories;
using LeoMadeiras.Application.Contracts.Services;
using LeoMadeiras.Application.UseCases.Auth.RegistrarUsuario;
using LeoMadeiras.Application.ViewModels.Auth.Request;
using LeoMadeiras.Domain.Entities;
using LeoMadeiras.Domain.Exceptions;
using Moq;

namespace LeoMadeiras.Tests.UseCases.Auth
{
    public class RegistrarUsuarioUseCaseTests
    {
        private readonly Mock<IUsuarioRepository> _repoMock = new();
        private readonly Mock<IJwtService> _jwtMock = new();
        private readonly Mock<IUnitOfWork> _uowMock = new();

        private RegistrarUsuarioUseCase CriarUseCase() =>
            new(_repoMock.Object, _jwtMock.Object, _uowMock.Object);

        [Fact]
        public async Task ExecuteAsync_EmailNovo_CriaUsuarioERetornaToken()
        {
            _repoMock
                .Setup(r => r.ExisteEmailAsync("novo@email.com", default))
                .ReturnsAsync(false);

            _jwtMock.Setup(j => j.GerarToken(It.IsAny<Usuario>())).Returns("token_jwt");
            _jwtMock.Setup(j => j.ObterExpiracao()).Returns(DateTime.UtcNow.AddHours(1));

            var response = await CriarUseCase().ExecuteAsync(new RegistrarUsuarioRequest
            {
                Nome = "Novo",
                Email = "novo@email.com",
                Senha = "senha123"
            });

            Assert.Equal("token_jwt", response.Token);
            Assert.Equal("Novo", response.Nome);
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Usuario>(), default), Times.Once);
            _uowMock.Verify(u => u.CommitAsync(default), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_EmailDuplicado_LancaDomainException()
        {
            _repoMock
                .Setup(r => r.ExisteEmailAsync("existente@email.com", default))
                .ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<DomainException>(() =>
                CriarUseCase().ExecuteAsync(new RegistrarUsuarioRequest
                {
                    Nome = "Leo",
                    Email = "existente@email.com",
                    Senha = "senha123"
                }));

            Assert.Contains("já cadastrado", ex.Message);
        }

        [Fact]
        public async Task ExecuteAsync_SenhaArmazenadaComoHash()
        {
            Usuario? usuarioCriado = null;

            _repoMock
                .Setup(r => r.ExisteEmailAsync(It.IsAny<string>(), default))
                .ReturnsAsync(false);

            _repoMock
                .Setup(r => r.AddAsync(It.IsAny<Usuario>(), default))
                .Callback<Usuario, CancellationToken>((u, _) => usuarioCriado = u);

            _jwtMock.Setup(j => j.GerarToken(It.IsAny<Usuario>())).Returns("token");
            _jwtMock.Setup(j => j.ObterExpiracao()).Returns(DateTime.UtcNow.AddHours(1));

            await CriarUseCase().ExecuteAsync(new RegistrarUsuarioRequest
            {
                Nome = "Leo",
                Email = "leo@email.com",
                Senha = "senha123"
            });

            Assert.NotNull(usuarioCriado);
            Assert.NotEqual("senha123", usuarioCriado!.SenhaHash);
            Assert.True(BCrypt.Net.BCrypt.Verify("senha123", usuarioCriado.SenhaHash));
        }
    }
}