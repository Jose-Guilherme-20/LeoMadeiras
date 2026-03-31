
using System;
using LeoMadeiras.Application.Contracts.Repositories;
using LeoMadeiras.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace LeoMadeiras.Infrastructure.Repositories
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext Context;
        protected readonly DbSet<T> DbSet;

        protected BaseRepository(AppDbContext context)
        {
            Context = context;
            DbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
            => await DbSet.FindAsync(new object[] { id }, ct);

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
            => await DbSet.AsNoTracking().ToListAsync(ct);

        public async Task AddAsync(T entity, CancellationToken ct = default)
            => await DbSet.AddAsync(entity, ct);

        public Task UpdateAsync(T entity, CancellationToken ct = default)
        {
            entity.SetUpdatedAt();
            DbSet.Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(T entity, CancellationToken ct = default)
        {
            DbSet.Remove(entity);
            return Task.CompletedTask;
        }
    }
