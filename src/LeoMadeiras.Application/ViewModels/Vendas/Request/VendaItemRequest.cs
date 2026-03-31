
using System.ComponentModel.DataAnnotations;

namespace LeoMadeiras.Application.ViewModels.Vendas.Request
{
    public class VendaItemRequest
    {
        [Required(ErrorMessage = "ProdutoId é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "ProdutoId inválido.")]
        public int ProdutoId { get; set; }

        [Required(ErrorMessage = "Quantidade é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero.")]
        public int Quantidade { get; set; }

        [Required(ErrorMessage = "Valor unitário é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor unitário deve ser maior que zero.")]
        public decimal ValorUnitario { get; set; }
    }
}
