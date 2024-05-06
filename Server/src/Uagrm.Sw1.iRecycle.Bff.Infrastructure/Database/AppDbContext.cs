using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Uagrm.Sw1.iRecycle.Bff.SharedKernel.Audit;

namespace Uagrm.Sw1.iRecycle.Bff.Infrastructure.Database;

public class AppDbContext : DbContext
{

    private readonly IHttpContextAccessor _httpContextAccesor;

    public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccesor = httpContextAccessor;
    }


    // Entity Definitions
    //public DbSet<User> Users => Set<User>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("dbo");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }


    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetAuditableEntityFields();
        return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }


    public override int SaveChanges() => SaveChangesAsync().GetAwaiter().GetResult();



    private void SetAuditableEntityFields()
    {
        string userName = "Default";
        if (_httpContextAccesor is not null
           && _httpContextAccesor.HttpContext is not null
           && _httpContextAccesor.HttpContext.User is not null
           && _httpContextAccesor.HttpContext.User.Identity is not null
           && _httpContextAccesor.HttpContext.User.Identity.IsAuthenticated)
        {
            userName = _httpContextAccesor!.HttpContext.User.Identity!.Name ?? "Default";
        }

        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Modified:
                    entry.Entity.LastModifiedBy = userName;
                    entry.Entity.LastModifiedAt = DateTime.Now;
                    break;

                case EntityState.Added:
                    entry.Entity.CreatedBy = userName;
                    entry.Entity.CreatedAt = DateTime.Now;
                    break;
            }
        }

    }

}
