
using LeoMadeiras.Domain.Common;

namespace LeoMadeiras.Domain.Entities
{
    public class Venda : BaseEntity
    {
        public Guid Order { get; private set; }
        public string Status { get; private set; } = string.Empty;
        public decimal Total { get; private set; }
        public DateTime DataVenda { get; private set; }

        private readonly List<VendaItem> _itens = new();
        public IReadOnlyCollection<VendaItem> Itens => _itens.AsReadOnly();

        protected Venda() { }

        public Venda(Guid order, string status, IEnumerable<VendaItem> itens)
        {
            Order = order;
            Status = status;
            DataVenda = DateTime.UtcNow;
            _itens.AddRange(itens);
            Total = _itens.Sum(i => i.ValorUnitario * i.Quantidade);
        }
    }
}
