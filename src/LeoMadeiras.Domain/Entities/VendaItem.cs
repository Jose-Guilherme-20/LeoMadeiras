
using LeoMadeiras.Domain.Common;

namespace LeoMadeiras.Domain.Entities
{
    public class VendaItem : BaseEntity
    {
        public int ProdutoId { get; private set; }
        public int Quantidade { get; private set; }
        public decimal ValorUnitario { get; private set; }
        public int VendaId { get; private set; }

        public Produto? Produto { get; private set; }

        protected VendaItem() { }

        public VendaItem(Produto produto, int quantidade, decimal valorUnitario)
        {
            Produto = produto;
            Quantidade = quantidade;
            ValorUnitario = valorUnitario;
        }
    }
}