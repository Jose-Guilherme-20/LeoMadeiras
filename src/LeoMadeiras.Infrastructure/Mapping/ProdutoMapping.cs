
using LeoMadeiras.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeoMadeiras.Infrastructure.Mapping
{
    public class ProdutoMapping : IEntityTypeConfiguration<Produto>
    {
        public void Configure(EntityTypeBuilder<Produto> builder)
        {
            builder.ToTable("Produtos");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nome)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(p => p.Descricao)
                .HasMaxLength(1000);

            builder.Property(p => p.Preco)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(p => p.QuantidadeEstoque)
                .IsRequired();

            builder.Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(p => p.UpdatedAt)
                .IsRequired(false);

            builder.Property<byte[]>("RowVersion")
                .IsRowVersion();
        }
    }
}
