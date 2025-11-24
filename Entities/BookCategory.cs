using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Libraray.Api.Entities;

[Table("BOOK_CATEGORIES")]
public partial class BookCategory
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("book_id")]
    public Guid BookId { get; set; }

    [Column("category_id")]
    public Guid CategoryId { get; set; }

    [ForeignKey("BookId")]
    [InverseProperty("BookCategories")]
    public virtual Book Book { get; set; } = null!;

    [ForeignKey("CategoryId")]
    [InverseProperty("BookCategories")]
    public virtual Category Category { get; set; } = null!;
}
