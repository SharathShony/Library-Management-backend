using Libraray.Api.Helpers.StoredProcedures;
using Libraray.Api.Entities;
using System.Data;

namespace Libraray.Api.Mappers.AdminMappers
{
    public static class GetBorrowingByIdWithDetailsMapper
    {
      /// <summary>
        /// Maps borrowingId parameter for stored procedure
 /// </summary>
   public static StoredProcedureParams<Guid> Parameters(Guid borrowingId) =>
       new StoredProcedureParams<Guid>("sp_get_borrowing_by_id_with_details").AddInputParameter("p_borrowing_id", borrowingId, DbType.Guid);

        /// <summary>
        /// Maps database reader to Borrowing entity with Book and User details
        /// </summary>
public static Func<IDataReader, BorrowingWithDetails> ResultMapper() => reader => new BorrowingWithDetails
  {
            // Borrowing fields
 Id = reader.GetGuid(reader.GetOrdinal("id")),
    UserId = reader.GetGuid(reader.GetOrdinal("user_id")),
   BookId = reader.GetGuid(reader.GetOrdinal("book_id")),
     BorrowDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("borrow_date"))),
   DueDate = reader.IsDBNull(reader.GetOrdinal("due_date")) 
? null 
     : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("due_date"))),
     ReturnDate = reader.IsDBNull(reader.GetOrdinal("return_date")) 
    ? null 
       : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("return_date"))),
   Status = reader.GetString(reader.GetOrdinal("status")),
       ApprovedBy = reader.IsDBNull(reader.GetOrdinal("approved_by")) 
    ? null 
         : reader.GetGuid(reader.GetOrdinal("approved_by")),
    ApprovedAt = reader.IsDBNull(reader.GetOrdinal("approved_at")) 
    ? null 
     : reader.GetDateTime(reader.GetOrdinal("approved_at")),
 RejectionReason = reader.IsDBNull(reader.GetOrdinal("rejection_reason")) 
   ? null 
   : reader.GetString(reader.GetOrdinal("rejection_reason")),
       
   // Book fields
    BookTitle = reader.GetString(reader.GetOrdinal("book_title")),
 BookAvailableCopies = reader.GetInt32(reader.GetOrdinal("book_available_copies")),
            
   // User fields
    UserUsername = reader.GetString(reader.GetOrdinal("user_username")),
   UserEmail = reader.GetString(reader.GetOrdinal("user_email"))
  };
    }

    /// <summary>
  /// DTO that includes Borrowing with nested Book and User details
    /// </summary>
    public class BorrowingWithDetails
 {
     // Borrowing properties
        public Guid Id { get; set; }
   public Guid UserId { get; set; }
        public Guid BookId { get; set; }
  public DateOnly BorrowDate { get; set; }
 public DateOnly? DueDate { get; set; }
    public DateOnly? ReturnDate { get; set; }
 public string Status { get; set; } = "pending";
public Guid? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? RejectionReason { get; set; }
    
   // Book properties
     public string BookTitle { get; set; } = string.Empty;
        public int BookAvailableCopies { get; set; }
   
      // User properties
  public string UserUsername { get; set; } = string.Empty;
      public string UserEmail { get; set; } = string.Empty;

        /// <summary>
  /// Converts to Borrowing entity with nested Book and User objects
/// </summary>
     public Borrowing ToBorrowing()
        {
   return new Borrowing
            {
    Id = this.Id,
      UserId = this.UserId,
     BookId = this.BookId,
  BorrowDate = this.BorrowDate,
   DueDate = this.DueDate,
    ReturnDate = this.ReturnDate,
   Status = this.Status,
   ApprovedBy = this.ApprovedBy,
    ApprovedAt = this.ApprovedAt,
         RejectionReason = this.RejectionReason,
 Book = new Book 
       { 
    Id = this.BookId,
   Title = this.BookTitle,
   AvailableCopies = this.BookAvailableCopies,
     UpdatedAt = DateTime.UtcNow
         },
          User = new User 
  { 
         Id = this.UserId,
   Username = this.UserUsername,
         Email = this.UserEmail
     }
   };
  }
    }
}
