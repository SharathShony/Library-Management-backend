namespace Libraray.Api.DTO.Admin
{
    public class OverdueBookDetailDto
    {
        public Guid BorrowingId { get; set; }
        public Guid BookId { get; set; }
        public string BookTitle { get; set; } = null!;
        public DateTime BorrowedDate { get; set; }
        public DateTime DueDate { get; set; }
        public int DaysOverdue { get; set; }
    }
}
