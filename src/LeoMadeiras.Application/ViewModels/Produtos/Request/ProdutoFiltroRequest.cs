
using System.ComponentModel.DataAnnotations;

namespace LeoMadeiras.Application.ViewModels.Produtos.Request
{
    public class ProdutoFiltroRequest
    {
        public string? Nome { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Preço mínimo não pode ser negativo.")]
        public decimal? PrecoMin { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Preço máximo não pode ser negativo.")]
        public decimal? PrecoMax { get; set; }

        public string? OrderBy { get; set; } = "nome";

        [Range(1, int.MaxValue, ErrorMessage = "Página deve ser maior que zero.")]
        public int Page { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "PageSize deve ser entre 1 e 100.")]
        public int PageSize { get; set; } = 10;
    }
}
