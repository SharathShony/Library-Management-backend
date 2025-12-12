namespace Libraray.Api.DTO.Admin
{
    public class UserOverdueBooksDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public List<OverdueBookDetailDto> OverdueBooks { get; set; } = new();
    }
}
