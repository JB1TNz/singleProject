using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace singleProject.Models.Db;

public partial class EbookBestContext : DbContext
{
    public EbookBestContext()
    {
    }

    public EbookBestContext(DbContextOptions<EbookBestContext> options)
        : base(options)
    {
    }

    public virtual DbSet<UserDatum> UserData { get; set; }

    public virtual DbSet<ProductData> Products { get; set; }

    public virtual DbSet<UserLibrary> UserLibraries { get; set; }

    public virtual DbSet<SupportTicket> SupportTickets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=EBookBest;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Thai_CI_AS");

        modelBuilder.Entity<UserDatum>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__UserData__1788CC4CFFC6D291");

            entity.Property(e => e.UserId).HasMaxLength(10);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.UserName).HasMaxLength(50);
            entity.Property(e => e.UserPassword).HasMaxLength(30);
            entity.Property(e => e.UserRole).HasMaxLength(20);
        });

        modelBuilder.Entity<ProductData>(entity =>
        {
            entity.HasKey(e => e.ProductId);
            entity.ToTable("Products");

            entity.Property(e => e.ProductId).ValueGeneratedOnAdd();
            entity.Property(e => e.ProductName).HasMaxLength(200);
            entity.Property(e => e.ProductDescription).HasMaxLength(2000);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.CoverPicture).HasMaxLength(500);
            entity.Property(e => e.SellerId).HasMaxLength(10);
        });

        modelBuilder.Entity<UserLibrary>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("UserLibrary");
            entity.Property(e => e.UserId).HasMaxLength(10).IsRequired();
            entity.Property(e => e.ProductId).IsRequired();
            entity.Property(e => e.PurchaseDate).HasDefaultValueSql("GETDATE()");
        });

        modelBuilder.Entity<SupportTicket>(entity =>
        {
            entity.HasKey(e => e.TicketId);
            entity.ToTable("SupportTickets");
            entity.Property(e => e.TicketId).ValueGeneratedOnAdd();
            entity.Property(e => e.UserId).HasMaxLength(10);
            entity.Property(e => e.Topic).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("Open");
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
