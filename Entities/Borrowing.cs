using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Libraray.Api.Entities;

[Table("BORROWINGS")]
public partial class Borrowing
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("book_id")]
    public Guid BookId { get; set; }

    [Column("borrow_date")]
    public DateOnly BorrowDate { get; set; }

    [Column("due_date")]
    public DateOnly? DueDate { get; set; }

    [Column("return_date")]
    public DateOnly? ReturnDate { get; set; }

    [Column("status")]
    [StringLength(50)]
    public string Status { get; set; } = "pending"; // Default to pending

    // Approval tracking fields
    [Column("approved_by")]
    public Guid? ApprovedBy { get; set; }

    [Column("approved_at")]
    public DateTime? ApprovedAt { get; set; }

    [Column("rejection_reason")]
    [StringLength(500)]
    public string? RejectionReason { get; set; }

    [ForeignKey("BookId")]
    [InverseProperty("Borrowings")]
    public virtual Book Book { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Borrowings")]
    public virtual User User { get; set; } = null!;
}
