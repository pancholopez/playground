﻿using Microsoft.EntityFrameworkCore;
using Multitenancy.Api.Services;

namespace Multitenancy.Api.Models;

public class ApplicationDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;
    private static readonly Guid TenantIdLoremIpsum = Guid.Parse("550e8400-e29b-41d4-a716-446655440000");

    public DbSet<UserAccount> UserAccounts { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Tenant> Tenants { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    // setup entity framework db schema and constrains
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Tenant>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();

        // setup query filter
        modelBuilder.Entity<Product>().HasQueryFilter(x => x.TenantId == _tenantContext.TenantId);
        modelBuilder.Entity<UserAccount>().HasQueryFilter(x => x.TenantId == _tenantContext.TenantId);

        // seed data
        modelBuilder.Entity<Tenant>().HasData(new Tenant
        {
            Id = TenantIdLoremIpsum,
            Name = "LoremIpsum company"
        });
        
        modelBuilder.Entity<UserAccount>().HasData(new UserAccount
            {
                Id = 1,
                Email = "user@example.com",
                Password = "password",
                TenantId = TenantIdLoremIpsum
            });
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
                    entry.Entity.TenantId = _tenantContext.TenantId;
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