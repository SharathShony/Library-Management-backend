namespace Libraray.Api.DTO.Books
{
    public class BorrowingHistoryDto
    {
        public Guid BorrowingId { get; set; }
        public Guid BookId { get; set; }
        public string BookTitle { get; set; } = null!;
        public string Author { get; set; } = null!;
        public DateOnly BorrowDate { get; set; }
        public DateOnly? DueDate { get; set; }
        public DateOnly? ReturnDate { get; set; }
        public string Status { get; set; } = null!;
        public bool WasOverdue { get; set; }
    }
}
