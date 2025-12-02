namespace Libraray.Api.DTO.Books
{
    public class ReturnBookResponse
    {
        public Guid BorrowingId { get; set; }
        public Guid BookId { get; set; }
        public DateOnly ReturnDate { get; set; }
        public int AvailableCopies { get; set; }
        public string Message { get; set; } = null!;
    }
}
