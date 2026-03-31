
using LeoMadeiras.Domain.Common;

namespace LeoMadeiras.Domain.Entities
{
    public class VendaItem : BaseEntity
    {
        public int Id { get; private set; }
        public int ProdutoId { get; private set; }
        public int Quantidade { get; private set; }
        public decimal ValorUnitario { get; private set; }
        public int VendaId { get; private set; }

        protected VendaItem() { }

        public VendaItem(int produtoId, int quantidade, decimal valorUnitario)
        {
            ProdutoId = produtoId;
            Quantidade = quantidade;
            ValorUnitario = valorUnitario;
        }
    }
