namespace Libraray.Api.DTOs.Books
{
    public class BookCatalogDto
    {
        public Guid BookId { get; set; }
        public string Title { get; set; } = null!;
        public string? Subtitle { get; set; }
        public string Publisher { get; set; } = null!;
        public string? Isbn { get; set; }
        public List<string> Authors { get; set; } = new();
        public List<string> Categories { get; set; } = new();
        public bool IsAvailable { get; set; }
    }
}
