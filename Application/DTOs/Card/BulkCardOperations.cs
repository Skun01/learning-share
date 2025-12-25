namespace Application.DTOs.Card;

public class BulkOperationError
{
    public int Index { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class BulkCreateCardsRequest
{
    public List<CreateCardRequest> Cards { get; set; } = new();
}

public class BulkCreateCardsResponse
{
    public int TotalRequested { get; set; }
    public int TotalCreated { get; set; }
    public List<CardDetailDTO> CreatedCards { get; set; } = new();
    public List<BulkOperationError>? Errors { get; set; }
}

public class BulkDeleteCardsRequest
{
    public List<int> CardIds { get; set; } = new();
}

public class BulkDeleteCardsResponse
{
    public int TotalRequested { get; set; }
    public int TotalDeleted { get; set; }
    public List<int> DeletedIds { get; set; } = new();
    public List<int>? FailedIds { get; set; }
}
