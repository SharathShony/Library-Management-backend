namespace Libraray.Api.DTO.Books
{
    public class BorrowBookResponse
    {
        public int AvailableCopies { get; set; }
        public Guid BorrowingId { get; set; }
    }
}