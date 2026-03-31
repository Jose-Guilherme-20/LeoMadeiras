
using System.ComponentModel.DataAnnotations;

namespace LeoMadeiras.Application.ViewModels.Produtos.Request
{
    public class AtualizarProdutoRequest
    {
        [Required(ErrorMessage = "Nome é obrigatório.")]
        [MaxLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [MaxLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres.")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "Preço é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que zero.")]
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "Quantidade em estoque é obrigatória.")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantidade não pode ser negativa.")]
        public int QuantidadeEstoque { get; set; }
    }
}
