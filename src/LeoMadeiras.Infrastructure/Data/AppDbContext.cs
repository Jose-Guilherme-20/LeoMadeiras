
using LeoMadeiras.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeoMadeiras.Infrastructure.Data
{
    public class AppDbContext : DbContext, IUnitOfWork
    {
        public DbSet<Produto> Produtos => Set<Produto>();
        public DbSet<Venda> Vendas => Set<Venda>();
        public DbSet<VendaItem> VendaItens => Set<VendaItem>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public Task<int> CommitAsync(CancellationToken ct = default) => SaveChangesAsync(ct);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Aplica automaticamente todos os IEntityTypeConfiguration da Assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
