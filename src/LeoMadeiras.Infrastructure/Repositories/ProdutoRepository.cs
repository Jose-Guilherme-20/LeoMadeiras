
using System;
using LeoMadeiras.Application.Contracts.Repositories;
using LeoMadeiras.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeoMadeiras.Infrastructure.Repositories
{
    public class ProdutoRepository : BaseRepository<Produto>, IProdutoRepository
    {
        public ProdutoRepository(AppDbContext context) : base(context) { }

        public async Task<PagedResult<Produto>> GetPagedAsync(ProdutoFiltroDto filtro, CancellationToken ct = default)
        {
            var query = DbSet.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro.Nome))
                query = query.Where(p => p.Nome.Contains(filtro.Nome));

            if (filtro.PrecoMin.HasValue)
                query = query.Where(p => p.Preco >= filtro.PrecoMin.Value);

            if (filtro.PrecoMax.HasValue)
                query = query.Where(p => p.Preco <= filtro.PrecoMax.Value);

            query = filtro.OrderBy?.ToLower() switch
            {
                "preco" => query.OrderBy(p => p.Preco),
                "preco_desc" => query.OrderByDescending(p => p.Preco),
                _ => query.OrderBy(p => p.Nome)
            };

            var total = await query.CountAsync(ct);
            var items = await query
                .Skip((filtro.Page - 1) * filtro.PageSize)
                .Take(filtro.PageSize)
                .ToListAsync(ct);

            return new PagedResult<Produto>(items, total, filtro.Page, filtro.PageSize);
        }

        public async Task<IEnumerable<MaisVendidoDto>> GetMaisVendidosAsync(CancellationToken ct = default)
            => await Context.VendaItens
                .GroupBy(i => new { i.ProdutoId })
                .Select(g => new MaisVendidoDto(
                    g.Key.ProdutoId,
                    g.First().Produto!.Nome,
                    g.Sum(i => i.Quantidade)
                ))
                .OrderByDescending(x => x.TotalVendido)
                .ToListAsync(ct);
    }
}
