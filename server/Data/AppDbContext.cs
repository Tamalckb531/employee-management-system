using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Spouse> Spouses => Set<Spouse>();
    public DbSet<Child> Children => Set<Child>();
    public DbSet<Admin> Admins => Set<Admin>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Employee
        modelBuilder.Entity<Employee>(e =>
        {
            e.HasIndex(x => x.NID).IsUnique();
            e.HasIndex(x => x.Phone).IsUnique();
            e.Property(x => x.BasicSalary).HasColumnType("decimal(18,2)");
        });

        // Spouse — one-to-one with Employee
        modelBuilder.Entity<Spouse>(s =>
        {
            s.HasIndex(x => x.NID).IsUnique();
            s.HasIndex(x => x.EmployeeId).IsUnique();
            s.HasOne(x => x.Employee)
             .WithOne(x => x.Spouse)
             .HasForeignKey<Spouse>(x => x.EmployeeId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // Child — many-to-one with Employee
        modelBuilder.Entity<Child>(c =>
        {
            c.HasOne(x => x.Employee)
             .WithMany(x => x.Children)
             .HasForeignKey(x => x.EmployeeId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // Admin
        modelBuilder.Entity<Admin>(a =>
        {
            a.HasIndex(x => x.Username).IsUnique();
        });
    }
}
