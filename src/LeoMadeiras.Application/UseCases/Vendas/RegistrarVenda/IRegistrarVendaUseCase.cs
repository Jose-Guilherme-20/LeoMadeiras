
using LeoMadeiras.Application.ViewModels.Vendas.Request;
using LeoMadeiras.Application.ViewModels.Vendas.Response;

namespace LeoMadeiras.Application.UseCases.Vendas.RegistrarVenda
{
    public interface IRegistrarVendaUseCase
    {
        Task<VendaResponse> ExecuteAsync(CriarVendaRequest request, CancellationToken ct = default);
    }
}
