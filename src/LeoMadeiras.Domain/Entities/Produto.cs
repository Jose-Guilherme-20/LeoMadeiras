
using LeoMadeiras.Domain.Common;

namespace LeoMadeiras.Domain.Entities
{
    public class Produto : BaseEntity
    {
        public string Nome { get; private set; } = string.Empty;
        public string Descricao { get; private set; } = string.Empty;
        public decimal Preco { get; private set; }
        public int QuantidadeEstoque { get; private set; }
        public DateTime DataCadastro { get; private set; }

        protected Produto() { }

        public Produto(string nome, string descricao, decimal preco, int quantidade)
        {
            Nome = nome;
            Descricao = descricao;
            Preco = preco;
            QuantidadeEstoque = quantidade;
            DataCadastro = DateTime.UtcNow;
        }

        public void Atualizar(string nome, string descricao, decimal preco, int quantidade)
        {
            Nome = nome;
            Descricao = descricao;
            Preco = preco;
            QuantidadeEstoque = quantidade;
        }

        public void DebitarEstoque(int quantidade)
        {
            if (quantidade <= 0)
                throw new DomainException("Quantidade deve ser maior que zero.");
            if (quantidade > QuantidadeEstoque)
                throw new DomainException($"Estoque insuficiente. Disponível: {QuantidadeEstoque}.");

            QuantidadeEstoque -= quantidade;
        }
    }
}
