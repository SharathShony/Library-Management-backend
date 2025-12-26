namespace Libraray.Api.DTO.Books
{
    public class BorrowedBookDto
    {
        public Guid BorrowingId { get; set; }
        public Guid BookId { get; set; }
        public string BookTitle { get; set; } = null!;
        public string Author { get; set; } = null!;
        public string? Isbn { get; set; }
        public string? Publisher { get; set; }
        public string? Summary { get; set; }
        public DateOnly BorrowDate { get; set; }
        public DateOnly DueDate { get; set; }
        public bool IsOverdue { get; set; }
    }
}
