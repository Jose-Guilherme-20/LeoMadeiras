
using LeoMadeiras.Application.Contracts;
using LeoMadeiras.Application.Contracts.Repositories;
using LeoMadeiras.Application.Contracts.Services;
using LeoMadeiras.Application.ViewModels.Auth.Request;
using LeoMadeiras.Application.ViewModels.Auth.Response;
using LeoMadeiras.Domain.Entities;
using LeoMadeiras.Domain.Exceptions;

namespace LeoMadeiras.Application.UseCases.Auth.RegistrarUsuario
{
    public class RegistrarUsuarioUseCase : IRegistrarUsuarioUseCase
    {
        private readonly IUsuarioRepository _repo;
        private readonly IJwtService _jwtService;
        private readonly IUnitOfWork _uow;

        public RegistrarUsuarioUseCase(
            IUsuarioRepository repo,
            IJwtService jwtService,
            IUnitOfWork uow)
        {
            _repo = repo;
            _jwtService = jwtService;
            _uow = uow;
        }

        public async Task<AuthResponse> ExecuteAsync(
            RegistrarUsuarioRequest request, CancellationToken ct = default)
        {
            if (await _repo.ExisteEmailAsync(request.Email, ct))
                throw new DomainException("E-mail já cadastrado.");

            var senhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha);
            var usuario = new Usuario(request.Nome, request.Email, senhaHash);

            await _repo.AddAsync(usuario, ct);
            await _uow.CommitAsync(ct);

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
