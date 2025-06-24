namespace api.DTOs
{
       public class RegisterDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Position { get; set; }
        public string? Team { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}