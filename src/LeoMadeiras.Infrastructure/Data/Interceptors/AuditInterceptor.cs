
using LeoMadeiras.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LeoMadeiras.Infrastructure.Data.Interceptors
{
    public class AuditInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            ApplyAudit(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken ct = default)
        {
            ApplyAudit(eventData.Context);
            return base.SavingChangesAsync(eventData, result, ct);
        }

        private static void ApplyAudit(DbContext? context)
        {
            if (context is null) return;

            var entries = context.ChangeTracker
                .Entries<BaseEntity>()
                .Where(e => e.State is EntityState.Added or EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                    entry.Entity.SetCreatedAt();

                if (entry.State == EntityState.Modified)
                    entry.Entity.SetUpdatedAt();
            }
        }
    }
}
