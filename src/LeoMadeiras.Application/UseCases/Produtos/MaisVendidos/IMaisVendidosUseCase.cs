
using LeoMadeiras.Application.ViewModels.Produtos.Response;

namespace LeoMadeiras.Application.UseCases.Produtos.MaisVendidos
{
    public interface IMaisVendidosUseCase
    {
        Task<IEnumerable<MaisVendidoResponse>> ExecuteAsync(CancellationToken ct = default);
    }
}
