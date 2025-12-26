namespace Libraray.Api.DTO.Books
{
    public class BorrowBookRequest
    {
        public Guid UserId { get; set; }
        public DateTime? DueDate { get; set; }
    }
}