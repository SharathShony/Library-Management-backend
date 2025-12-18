namespace Libraray.Api.DTO.Admin
{
    public class OverdueUserDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int OverdueCount { get; set; }
    }
}
