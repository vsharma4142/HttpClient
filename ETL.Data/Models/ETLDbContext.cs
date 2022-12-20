using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ETL.Data.Models;

public partial class ETLDbContext : DbContext
{
    public ETLDbContext()
    {
    }

    public ETLDbContext(DbContextOptions<ETLDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccountDetail> AccountDetails { get; set; }

    public virtual DbSet<AccountType> AccountTypes { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Code\\Salesforce\\BulkAPI\\ETL.API\\ETL.Data\\ETLDatabase.mdf;Integrated Security=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("AccountDetails");

            entity.Property(e => e.AccontCatageory)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Accont Catageory");
            entity.Property(e => e.AccountRegion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Account Region");
            entity.Property(e => e.AccountType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Account Type");
            entity.Property(e => e.CustomerName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Customer Name");
            entity.Property(e => e.CustomerRegion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Customer Region");
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Ssn)
                .HasMaxLength(20)
                .IsFixedLength()
                .HasColumnName("SSN");
        });

        modelBuilder.Entity<AccountType>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__AccountT__349DA5A67D97934E");

            entity.ToTable("AccountType");

            entity.Property(e => e.AccountId).ValueGeneratedNever();
            entity.Property(e => e.AccontCatageory)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Accont Catageory");
            entity.Property(e => e.AccountRegion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Account Region");
            entity.Property(e => e.AccountType1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Account Type");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64D87CF0AAFE");

            entity.Property(e => e.CustomerId).ValueGeneratedNever();
            entity.Property(e => e.CustomerName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Customer Name");
            entity.Property(e => e.CustomerRegion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Customer Region");
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Ssn)
                .HasMaxLength(20)
                .IsFixedLength()
                .HasColumnName("SSN");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
