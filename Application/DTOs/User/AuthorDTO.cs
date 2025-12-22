namespace Application.DTOs.User;

public record class AuthorDTO(
    int Id,
    string Name,
    string? AvatarUrl
);