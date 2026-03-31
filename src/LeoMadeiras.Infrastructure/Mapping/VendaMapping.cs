
using LeoMadeiras.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeoMadeiras.Infrastructure.Mapping
{
    public class VendaMapping : IEntityTypeConfiguration<Venda>
    {
        public void Configure(EntityTypeBuilder<Venda> builder)
        {
            builder.ToTable("Vendas");

            builder.HasKey(v => v.Id);

            builder.HasIndex(v => v.Order)
                .IsUnique();

            builder.Property(v => v.Order)
                .IsRequired();

            builder.Property(v => v.Status)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(v => v.Total)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(v => v.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(v => v.UpdatedAt)
                .IsRequired(false);

            builder.HasMany(v => v.Itens)
                .WithOne()
                .HasForeignKey(i => i.VendaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
