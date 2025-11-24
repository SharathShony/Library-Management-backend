using System;
using System.Collections.Generic;
using Libraray.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Libraray.Api.Data;

public partial class LibraryDbContext : DbContext
{
    public LibraryDbContext()
    {
    }

    public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<BookAuthor> BookAuthors { get; set; }

    public virtual DbSet<BookCategory> BookCategories { get; set; }

    public virtual DbSet<Borrowing> Borrowings { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=SHARATHSHONY\\SQLEXPRESS;Database=LMS;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AUTHORS__3213E83F8491C818");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BOOKS__3213E83FD1A000CE");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AvailableCopies).HasDefaultValue(1);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.TotalCopies).HasDefaultValue(1);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysdatetime())");
        });

        modelBuilder.Entity<BookAuthor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BOOK_AUT__3213E83FD7727256");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Author).WithMany(p => p.BookAuthors)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BOOK_AUTH__autho__4D94879B");

            entity.HasOne(d => d.Book).WithMany(p => p.BookAuthors)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BOOK_AUTH__book___4CA06362");
        });

        modelBuilder.Entity<BookCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BOOK_CAT__3213E83F7444D29E");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Book).WithMany(p => p.BookCategories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BOOK_CATE__book___5165187F");

            entity.HasOne(d => d.Category).WithMany(p => p.BookCategories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BOOK_CATE__categ__52593CB8");
        });

        modelBuilder.Entity<Borrowing>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BORROWIN__3213E83F982CA074");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.BorrowDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Book).WithMany(p => p.Borrowings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BORROWING__book___5812160E");

            entity.HasOne(d => d.User).WithMany(p => p.Borrowings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BORROWING__user___571DF1D5");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CATEGORI__3213E83F84B47EEF");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__USERS__3213E83F226027D8");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysdatetime())");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
