
using LeoMadeiras.Application.ViewModels.Auth.Request;
using LeoMadeiras.Application.ViewModels.Auth.Response;

namespace LeoMadeiras.Application.UseCases.Auth.Login
{
    public interface ILoginUseCase
    {
        Task<AuthResponse> ExecuteAsync(LoginRequest request, CancellationToken ct = default);
    }
}

