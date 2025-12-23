namespace Application.DTOs.User;

public class AuthorDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
}