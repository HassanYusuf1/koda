namespace api.DTOs
{
    public record RegisterDto(
        string Email,
        string Password,
        string FullName,
        string? Position,
        string? Team,
        DateTime? DateOfBirth,
        string Role);
}