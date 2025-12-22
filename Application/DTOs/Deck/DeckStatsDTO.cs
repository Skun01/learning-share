namespace Application.DTOs.Deck;

public record class DeckStatsDTO(
    int TotalCards,
    int Downloads,
    int Learned,
    double Progress,
    int CardsDue
);
