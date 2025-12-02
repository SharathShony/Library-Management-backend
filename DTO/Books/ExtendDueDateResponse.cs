namespace Libraray.Api.DTO.Books
{
    public class ExtendDueDateResponse
    {
        public Guid BorrowingId { get; set; }
   public DateOnly NewDueDate { get; set; }
        public int ExtensionDays { get; set; }
  public string Message { get; set; } = null!;
    }
}
