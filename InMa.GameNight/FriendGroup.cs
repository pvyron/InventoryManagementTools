namespace InMa.GameNight;

public sealed class FriendGroup
{
    private FriendGroup(Ulid ownerId, string name, string description)
    {
        Id = Ulid.NewUlid();
        OwnerId = ownerId;
        Name = name;
        Description = description;

        _friends = [];
        _vetoesPerFriend = [];
    }
    
    public Ulid Id { get; private init; }
    public Ulid OwnerId { get; private init; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    
    private List<Friend> _friends;
    public IReadOnlyCollection<Friend> Friends => _friends;
    
    private Dictionary<Ulid, int> _vetoesPerFriend;
    public IReadOnlyDictionary<Ulid, int> VetoesPerFriend => _vetoesPerFriend;
    
    public static FriendGroup Create(Ulid ownerId, string name, string description) =>
        new(ownerId, name, description);

    public void AddFriend(Friend friend, ushort availableVetoes)
    {
        var existingFriend = _friends.FirstOrDefault(f => f.Id == friend.Id);
        
        if (existingFriend is not null)
            _friends.Remove(existingFriend);
        
        _friends.Add(friend);

        if (!_vetoesPerFriend.TryAdd(friend.Id, availableVetoes))
        {
            _vetoesPerFriend[friend.Id] = Math.Max(_vetoesPerFriend[friend.Id], availableVetoes);
        }
    }

    public void UpdateFriendData(Friend friend)
    {
        var existingFriend = _friends.FirstOrDefault(f => f.Id == friend.Id);

        if (existingFriend is null)
            return;

        _friends.Remove(existingFriend);
        _friends.Add(friend);
    }
}