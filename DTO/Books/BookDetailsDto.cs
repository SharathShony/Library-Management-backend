namespace Libraray.Api.DTO.Books
{
    public class BookDetailsDto
    {
        public Guid BookId { get; set; }
        public string Title { get; set; } = null!;
        public string? Subtitle { get; set; }
        public string? Isbn { get; set; }
        public string? Summary { get; set; }
        public string? Publisher { get; set; }
        public DateOnly? PublicationDate { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public bool IsAvailable { get; set; }
        public List<string> Authors { get; set; } = new();
        public List<string> Categories { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
