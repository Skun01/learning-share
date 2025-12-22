using Application.DTOs.User;

namespace Application.DTOs.Deck;

public record class DeckSummaryDTO(
    int Id,
    string Name,
    string? Description,
    string Type,
    AuthorDTO Author,
    DeckStatsDTO Stats,
    List<string> Tags,
    bool IsPublic,
    int? SourceDeckId,
    DateTime CreatedAt
);
