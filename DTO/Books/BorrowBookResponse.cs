namespace Libraray.Api.DTO.Books
{
    public class BorrowBookResponse
    {
        public Guid BorrowingId { get; set; }
        public int AvailableCopies { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}