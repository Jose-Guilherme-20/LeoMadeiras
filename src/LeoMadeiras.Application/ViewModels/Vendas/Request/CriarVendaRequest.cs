
using System.ComponentModel.DataAnnotations;

namespace LeoMadeiras.Application.ViewModels.Vendas.Request
{
    public class CriarVendaRequest
    {
        [Required(ErrorMessage = "Order é obrigatório.")]
        public Guid Order { get; set; }

        [Required(ErrorMessage = "Status é obrigatório.")]
        [MaxLength(50, ErrorMessage = "Status deve ter no máximo 50 caracteres.")]
        public string Status { get; set; } = string.Empty;

        [Required(ErrorMessage = "A venda deve ter ao menos um item.")]
        [MinLength(1, ErrorMessage = "A venda deve ter ao menos um item.")]
        public List<VendaItemRequest> Itens { get; set; } = new();
    }
}
