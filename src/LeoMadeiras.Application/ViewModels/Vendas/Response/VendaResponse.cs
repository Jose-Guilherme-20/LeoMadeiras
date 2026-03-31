
namespace LeoMadeiras.Application.ViewModels.Vendas.Response
{
    public class VendaResponse
    {
        public int Id { get; set; }
        public Guid Order { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public List<VendaItemResponse> Itens { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}
