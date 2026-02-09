using System;
using System.Collections.Generic;
using Libraray.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Libraray.Api.Context;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AUTHORS__3213E83F8491C818");

            // 🔥 CHANGED: newid() → gen_random_uuid() for PostgreSQL
            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BOOKS__3213E83FD1A000CE");

            // 🔥 CHANGED: SQL Server functions → PostgreSQL functions
            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.AvailableCopies).HasDefaultValue(1);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP"); // sysdatetime() → CURRENT_TIMESTAMP
            entity.Property(e => e.TotalCopies).HasDefaultValue(1);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<BookAuthor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BOOK_AUT__3213E83FD7727256");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

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

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

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

            // 🔥 CHANGED: getdate() → CURRENT_TIMESTAMP for PostgreSQL
            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.BorrowDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

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

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__USERS__3213E83F226027D8");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
