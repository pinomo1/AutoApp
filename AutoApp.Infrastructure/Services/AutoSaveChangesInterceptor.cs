using AutoApp.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AutoApp.Infrastructure.Services;

public class AutoSaveChangesInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        var dbContext = eventData.Context;
        if (dbContext == null) return base.SavingChanges(eventData, result);
        foreach (var entry in dbContext.ChangeTracker.Entries()
                     .Where(e => e.State is EntityState.Added or EntityState.Modified))
        {
            if (entry.Entity is not AbstractModel auditable) continue;
            switch (entry.State)
            {
                case EntityState.Added:
                    var now = DateTime.UtcNow;
                    auditable.CreatedAt = now;
                    auditable.UpdatedAt = now;
                    break;
                case EntityState.Modified:
                    auditable.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        return base.SavingChanges(eventData, result);
    }
}