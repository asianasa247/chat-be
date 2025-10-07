using ManageEmployee.Dal.DbContexts;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ManageEmployee.DataLayer.Service.Test.TestUtils.DbContextBuilders;

internal class MockDbContext
{
    private readonly Dictionary<object, EntityState> _changeTracker = new();

    private ApplicationDbContext? _context;
    public static MockDbContext Create()
    {
        var mockDbContext = new MockDbContext();
        return mockDbContext.CreateDbContext();
    }

    public ApplicationDbContext GetDbContext()
    {
        return _context!;
    }

    private MockDbContext CreateDbContext()
    {
        _context = Substitute.For<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
        _context.SaveChanges().Returns(info =>
        {
            // Implement custom logic to count changes
            var changeCount = _changeTracker.Count(kv => kv.Value == EntityState.Added ||
                                                              kv.Value == EntityState.Modified ||
                                                              kv.Value == EntityState.Deleted);
            _changeTracker.Clear();
            return changeCount;
        });
        return this;
    }

    public MockDbContext MockDbSet<T>(List<T> entities) where T : class
    {
        var mockSet = Substitute.For<DbSet<T>, IQueryable<T>>();

        var list = new List<T>(entities);
        var queryable = list.AsQueryable();
        // Query the set

        ((IQueryable<T>)mockSet).Provider.Returns(queryable.Provider);
        ((IQueryable<T>)mockSet).Expression.Returns(queryable.Expression);
        ((IQueryable<T>)mockSet).ElementType.Returns(queryable.ElementType);
        ((IQueryable<T>)mockSet).GetEnumerator().Returns(info => queryable.GetEnumerator());

        // Modify the set
        mockSet.When(set => set.Add(Arg.Any<T>())).Do(info =>
        {
            list.Add(info.Arg<T>());
            queryable = list.AsQueryable();
            _changeTracker[info.Arg<T>()] = EntityState.Added;
        });

        mockSet.When(set => set.Remove(Arg.Any<T>())).Do(info =>
        {
            list.Remove(info.Arg<T>());
            queryable = list.AsQueryable();
            _changeTracker[info.Arg<T>()] = EntityState.Deleted;
        });

        mockSet.When(set => set.AddRange(Arg.Any<List<T>>())).Do(info =>
        {
            list.AddRange(info.Arg<List<T>>());
            queryable = list.AsQueryable();
            foreach (var item in info.Arg<List<T>>())
            {
                _changeTracker[item] = EntityState.Added;
            }
        });

        mockSet.When(set => set.RemoveRange(Arg.Any<List<T>>())).Do(info =>
        {
            foreach (var item in info.Arg<List<T>>())
            {
                list.Remove(item);
            }
            queryable = list.AsQueryable();
            foreach (var item in info.Arg<List<T>>())
            {
                _changeTracker[item] = EntityState.Deleted;
            }
        });

        foreach (var item in _context!.GetType().GetProperties())
        {
            if(item.PropertyType == typeof(DbSet<T>))
            {
                item.SetValue(_context, mockSet);
            }
        }

        return this;
    }
}