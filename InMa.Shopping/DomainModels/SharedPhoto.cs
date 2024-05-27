namespace InMa.Shopping.DomainModels;

public class SharedPhoto : Entity
{
    public required string Title { get; init; }
    public string? Description { get; set; }
    
    public required DateTime UploadedOn { get; set; }
    
    public Location? CapturedAt { get; set; }
    public DateOnly? CapturedOn { get; set; }
    
    public ICollection<string>? Tags { get; set; }
}