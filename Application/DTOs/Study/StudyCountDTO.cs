namespace Application.DTOs.Study;

public class StudyCountDTO
{
    public int Reviews { get; set; }   // Pending reviews (due now)
    public int New { get; set; }       // New cards to learn
    public int Ghosts { get; set; }    // Ghost cards for reinforcement
}
