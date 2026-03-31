
using System;
using LeoMadeiras.Application.Contracts.Repositories;
using LeoMadeiras.Application.ViewModels.Common;
using LeoMadeiras.Application.ViewModels.Produtos.Request;
using LeoMadeiras.Application.ViewModels.Produtos.Response;
using LeoMadeiras.Domain.Entities;
using LeoMadeiras.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LeoMadeiras.Infrastructure.Repositories
{
    public class ProdutoRepository : BaseRepository<Produto>, IProdutoRepository
    {
        public ProdutoRepository(AppDbContext context) : base(context) { }

        public async Task<PagedResultViewModel<Produto>> GetPagedAsync(
            ProdutoFiltroRequest filtro, CancellationToken ct = default)
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

            return new PagedResultViewModel<Produto>
            {
                Items = items,
                Total = total,
                Page = filtro.Page,
                PageSize = filtro.PageSize
            };
        }

        public async Task<IEnumerable<MaisVendidoResponse>> GetMaisVendidosAsync(CancellationToken ct = default)
            => await Context.VendaItens
                .AsNoTracking()
                .GroupBy(i => new { i.ProdutoId, i.Produto!.Nome })
                .Select(g => new MaisVendidoResponse
                {
                    ProdutoId = g.Key.ProdutoId,
                    Nome = g.Key.Nome,
                    TotalVendido = g.Sum(i => i.Quantidade)
                })
                .OrderByDescending(x => x.TotalVendido)
                .ToListAsync(ct);
    }
}

