using System.Collections.Generic;
using LeoMadeiras.Application.Contracts.Repositories;
using LeoMadeiras.Domain.Entities;
using LeoMadeiras.Infrastructure.Data;
using LeoMadeiras.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(AppDbContext context) : base(context) { }

    public Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default)
        => DbSet.FirstOrDefaultAsync(u => u.Email == email, ct);

    public Task<bool> ExisteEmailAsync(string email, CancellationToken ct = default)
        => DbSet.AnyAsync(u => u.Email == email, ct);
}