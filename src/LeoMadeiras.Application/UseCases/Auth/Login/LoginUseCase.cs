
using LeoMadeiras.Application.Contracts.Repositories;
using LeoMadeiras.Application.Contracts.Services;
using LeoMadeiras.Application.ViewModels.Auth.Request;
using LeoMadeiras.Application.ViewModels.Auth.Response;
using LeoMadeiras.Domain.Exceptions;

namespace LeoMadeiras.Application.UseCases.Auth.Login
{
    public class LoginUseCase : ILoginUseCase
    {
        private readonly IUsuarioRepository _repo;
        private readonly IJwtService _jwtService;

        public LoginUseCase(IUsuarioRepository repo, IJwtService jwtService)
        {
            _repo = repo;
            _jwtService = jwtService;
        }

        public async Task<AuthResponse> ExecuteAsync(
            LoginRequest request, CancellationToken ct = default)
        {
            var usuario = await _repo.GetByEmailAsync(request.Email, ct)
                ?? throw new DomainException("E-mail ou senha inválidos.");

            var senhaValida = BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash);
            if (!senhaValida)
                throw new DomainException("E-mail ou senha inválidos.");

            return new AuthResponse
            {
                Token = _jwtService.GerarToken(usuario),
                Nome = usuario.Nome,
                Email = usuario.Email,
                ExpiresAt = _jwtService.ObterExpiracao()
            };
        }
    }
}
