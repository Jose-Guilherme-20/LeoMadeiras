
namespace LeoMadeiras.Application.ViewModels.Produtos.Response
{
    public class MaisVendidoResponse
    {
        public int ProdutoId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int TotalVendido { get; set; }
    }
}
