namespace InMa.Shopping.DomainModels;

public readonly record struct EntityId
{
    private EntityId(Guid input)
    {
        _guid = input;
        _id = input.ToString("N");
    }
    
    private EntityId(string inputId)
    {
        _id = inputId;

        if (!Guid.TryParseExact(inputId, "N", out _guid))
        {
            throw new ArgumentException($"id: {inputId} is an invalid representation of a Guid", nameof(inputId));
        }
    }

    private readonly Guid _guid;
    private readonly string _id;

    public static EntityId New() => new(Guid.NewGuid());
    public static EntityId Existing(string id) => new(id);
    
    public static implicit operator string(EntityId listId) => listId._id;
    public static implicit operator Guid(EntityId listId) => listId._guid;

    public override string ToString() => _id;
}