namespace Application.DTOs.Study;

public class SubmitReviewRequest
{
    public bool IsCorrect { get; set; }
    public int? TimeSpentMs { get; set; }

    // === Thông tin chi tiết review ===
    public string? UserAnswer { get; set; }         // Câu trả lời của user
    public int? ExampleId { get; set; }             // Example nào được dùng
    public Guid? SessionId { get; set; }            // Session ID để nhóm reviews
    public string? ReviewType { get; set; }         // "Learn", "Review", "Cram", "Ghost"
}
