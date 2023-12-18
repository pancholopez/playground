using Microsoft.EntityFrameworkCore;
using Multitenancy.Api.Services;

namespace Multitenancy.Api.Models;

public class ApplicationDbContext : DbContext
{
    private readonly Guid _tenantId;
    public DbSet<Product> Products { get; set; }

    public DbSet<Tenant> Tenants { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantService tenantService)
        : base(options)
    {
        _tenantId = tenantService.GetTenantId();
    }

    // setup entity framework db schema and constrains
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Tenant>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();
        
        // setup global query filter
        modelBuilder.Entity<ITenantAware>().HasQueryFilter(x => x.TenantId == _tenantId);
    }

    // before saving changes, we make sure that entities that are aware of Tenant contain the current tenant Id
    private void SetEntityTenantId()
    {
        foreach (var entry in ChangeTracker.Entries<ITenantAware>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                case EntityState.Modified:
                    entry.Entity.TenantId = _tenantId;
                    break;
            }
        }
    }

    public override int SaveChanges()
    {
        SetEntityTenantId();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        SetEntityTenantId();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        SetEntityTenantId();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
    {
        SetEntityTenantId();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}