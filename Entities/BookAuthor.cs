using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Libraray.Api.Entities;

[Table("BOOK_AUTHORS")]
public partial class BookAuthor
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("book_id")]
    public Guid BookId { get; set; }

    [Column("author_id")]
    public Guid AuthorId { get; set; }

    [ForeignKey("AuthorId")]
    [InverseProperty("BookAuthors")]
    public virtual Author Author { get; set; } = null!;

    [ForeignKey("BookId")]
    [InverseProperty("BookAuthors")]
    public virtual Book Book { get; set; } = null!;
}
