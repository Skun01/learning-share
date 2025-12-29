namespace Application.DTOs.Study;

public class GetNewLessonsRequest
{
    public int DeckId { get; set; }
    public int Limit { get; set; } = 5;
}
