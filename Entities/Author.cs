using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Libraray.Api.Entities;

[Table("AUTHORS")]
public partial class Author
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("name")]
    [StringLength(150)]
    public string Name { get; set; } = null!;

    [Column("bio")]
    public string? Bio { get; set; }

    [InverseProperty("Author")]
    public virtual ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
}
