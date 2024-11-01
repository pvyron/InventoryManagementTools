namespace InMa.GameNight;

public sealed class FriendGroup
{
    private const ushort StartingVetoes = 2;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    /// <summary>
    /// Not to be used in code, only for automatic data retrival
    /// </summary>
    private FriendGroup()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        
    }
    
    private FriendGroup(Ulid ownerId, string name, string description)
    {
        Id = Ulid.NewUlid();
        OwnerId = ownerId;
        Name = name;
        Description = description;

        _friends = [];
        _vetoesPerFriend = [];
        _pastGameProposals = [];
    }
    
    public Ulid Id { get; private init; }
    public Ulid OwnerId { get; private init; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    
    public IReadOnlyCollection<Friend> Friends => _friends;
    private List<Friend> _friends;
    
    public IReadOnlyDictionary<Ulid, ushort> VetoesPerFriend => _vetoesPerFriend;
    private Dictionary<Ulid, ushort> _vetoesPerFriend;
    
    public GameProposal? ActiveProposal { get; private set; }
    
    public IReadOnlyCollection<GameProposal> PastGameProposals => _pastGameProposals;
    private List<GameProposal> _pastGameProposals;
    
    public static FriendGroup Create(Ulid ownerId, string name, string description) =>
        new(ownerId, name, description);

    public void AddFriend(Friend friend, ushort availableVetoes = StartingVetoes)
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

    public void UpdateFriendsVetoes(Friend friend, ushort availableVetoes)
    {
        _vetoesPerFriend[friend.Id] = availableVetoes;
    }

    public bool StartProposal(Friend proposer, VideoGame videoGame)
    {
        if (ActiveProposal is not null)
            return false;
        
        ActiveProposal = GameProposal.StartProposal(this, proposer, videoGame);

        return true;
    }

    public bool EndProposal(Friend player)
    {
        if (ActiveProposal is null)
            return false;

        if (player.Id != ActiveProposal.ProposerId)
            _vetoesPerFriend[player.Id]++;

        var endedProposal = ActiveProposal;
        endedProposal.EndSuccessfulProposal(player);
        _pastGameProposals.Add(endedProposal);
        
        ActiveProposal = null;
        return true;
    }

    public bool VetoProposal(Friend vetoer)
    {
        if (ActiveProposal is null)
            return false;

        if (vetoer.Id == ActiveProposal.ProposerId)
            return false;

        if (!_vetoesPerFriend.TryGetValue(vetoer.Id, out var vetoes))
            return false;
        
        if (vetoes == 0)
            return false;

        _vetoesPerFriend[vetoer.Id]--;

        var endedProposal = ActiveProposal;
        endedProposal.EndFailedProposal(vetoer);
        _pastGameProposals.Add(endedProposal);
        
        ActiveProposal = null;
        return true;
    }
}