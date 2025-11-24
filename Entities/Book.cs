using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Libraray.Api.Entities;

[Table("BOOKS")]
[Index("Isbn", Name = "UQ__BOOKS__99F9D0A4C46FB550", IsUnique = true)]
public partial class Book
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("title")]
    [StringLength(200)]
    public string Title { get; set; } = null!;

    [Column("subtitle")]
    [StringLength(200)]
    public string? Subtitle { get; set; }

    [Column("isbn")]
    [StringLength(50)]
    public string? Isbn { get; set; }

    [Column("summary")]
    public string? Summary { get; set; }

    [Column("publisher")]
    [StringLength(200)]
    public string? Publisher { get; set; }

    [Column("publication_date")]
    public DateOnly? PublicationDate { get; set; }

    [Column("total_copies")]
    public int TotalCopies { get; set; }

    [Column("available_copies")]
    public int AvailableCopies { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [InverseProperty("Book")]
    public virtual ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();

    [InverseProperty("Book")]
    public virtual ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();

    [InverseProperty("Book")]
    public virtual ICollection<Borrowing> Borrowings { get; set; } = new List<Borrowing>();
}
