using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace singleProject.Models;

public partial class Csi402dbContext : DbContext
{
    public Csi402dbContext()
    {
    }

    public Csi402dbContext(DbContextOptions<Csi402dbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Cartitem> Cartitems { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;port=3306;database=csi402db;user=root;password=123456789", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.17-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PRIMARY");

            entity.ToTable("carts");

            entity.Property(e => e.CartId)
                .HasMaxLength(100)
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
            entity.Property(e => e.UserId)
                .HasMaxLength(20)
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
        });

        modelBuilder.Entity<Cartitem>(entity =>
        {
            entity.HasKey(e => e.CartItemId).HasName("PRIMARY");

            entity.ToTable("cartitems");

            entity.Property(e => e.CartItemId)
                .HasMaxLength(20)
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
            entity.Property(e => e.CartId).HasColumnType("int(11)");
            entity.Property(e => e.ProductId).HasColumnType("int(11)");
            entity.Property(e => e.Quantity).HasColumnType("int(11)");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PRIMARY");

            entity.ToTable("products");

            entity.Property(e => e.ProductId)
                .HasMaxLength(100)
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
            entity.Property(e => e.Descriptions)
                .HasMaxLength(100)
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.Property(e => e.ProductName)
                .HasMaxLength(200)
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
            entity.Property(e => e.Stock).HasColumnType("int(11)");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("users");

            entity.Property(e => e.UserId)
                .HasMaxLength(20)
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
