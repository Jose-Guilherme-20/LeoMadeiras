
using System;
using LeoMadeiras.Application.Contracts.Repositories;
using LeoMadeiras.Domain.Entities;
using LeoMadeiras.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LeoMadeiras.Infrastructure.Repositories
{
    public class VendaRepository : BaseRepository<Venda>, IVendaRepository
    {
        public VendaRepository(AppDbContext context) : base(context) { }

        public Task<bool> ExisteOrderAsync(Guid order, CancellationToken ct = default)
            => Context.Vendas.AnyAsync(v => v.Order == order, ct);
    }
}
