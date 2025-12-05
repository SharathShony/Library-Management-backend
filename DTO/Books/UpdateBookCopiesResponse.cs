namespace Libraray.Api.DTO.Books
{
    public class UpdateBookCopiesResponse
    {
        public int TotalCopies { get; set; }
    public int AvailableCopies { get; set; }
      public string Message { get; set; } = null!;
    }
}
