
using LeoMadeiras.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeoMadeiras.Infrastructure.Mapping
{
    public class VendaItemMapping : IEntityTypeConfiguration<VendaItem>
    {
        public void Configure(EntityTypeBuilder<VendaItem> builder)
        {
            builder.ToTable("VendaItens");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.Quantidade)
                .IsRequired();

            builder.Property(i => i.ValorUnitario)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(i => i.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(i => i.UpdatedAt)
                .IsRequired(false);

            builder.HasOne(i => i.Produto)
                .WithMany()
                .HasForeignKey(i => i.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
