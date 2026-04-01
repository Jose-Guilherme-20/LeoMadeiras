
using LeoMadeiras.Application.ViewModels.Auth.Request;
using LeoMadeiras.Application.ViewModels.Auth.Response;

namespace LeoMadeiras.Application.UseCases.Auth.RegistrarUsuario
{
    public interface IRegistrarUsuarioUseCase
    {
        Task<AuthResponse> ExecuteAsync(RegistrarUsuarioRequest request, CancellationToken ct = default);
    }
}
