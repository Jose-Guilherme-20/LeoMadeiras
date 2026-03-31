
using LeoMadeiras.Application.Contracts.Repositories;
using LeoMadeiras.Application.ViewModels.Produtos.Response;

namespace LeoMadeiras.Application.UseCases.Produtos.MaisVendidos
{
    public class MaisVendidosUseCase : IMaisVendidosUseCase
    {
        private readonly IProdutoRepository _repo;

        public MaisVendidosUseCase(IProdutoRepository repo) => _repo = repo;

        public async Task<IEnumerable<MaisVendidoResponse>> ExecuteAsync(CancellationToken ct = default)
        {
            var result = await _repo.GetMaisVendidosAsync(ct);

            return result.Select(x => new MaisVendidoResponse
            {
                ProdutoId = x.ProdutoId,
                Nome = x.Nome,
                TotalVendido = x.TotalVendido
            });
        }
    }
}
