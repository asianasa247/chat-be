using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ManageEmployee.Dal.Interceptors;

public class UpdateYearInterceptor : SaveChangesInterceptor
{
    private const string YearProperty = "Year";

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            UpdateEntities(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void UpdateEntities(DbContext context)
    {
        try
        {
            var entities = ModifyEntries(context);

            foreach (EntityEntry entry in entities)
            {
                if (!IsYearProperty(entry, out var yearProperty))
                {
                    continue;
                }

                yearProperty.CurrentValue = DateTime.UtcNow.Year;
            }
        }
        catch (Exception ex)
        {
            Debug.Write(ex);
        }
    }

    private static bool IsYearProperty(EntityEntry entry, out PropertyEntry yearProperty)
    {
        yearProperty = entry.Properties.FirstOrDefault(x => x.Metadata.Name == YearProperty);
        return yearProperty is not null
           && (yearProperty.Metadata.ClrType == typeof(int)
            || yearProperty.Metadata.ClrType == typeof(int?))
           && int.Parse(yearProperty.CurrentValue.ToString()) == 0;
    }

    private static List<EntityEntry> ModifyEntries(DbContext context)
    {
        return context.ChangeTracker
            .Entries()
            .Where(x => x.State == EntityState.Added
                     || x.State == EntityState.Modified)
            .ToList();
    }
}
