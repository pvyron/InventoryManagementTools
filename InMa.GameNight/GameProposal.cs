namespace InMa.GameNight;

public sealed class GameProposal
{
    private GameProposal(Ulid friendGroupId, Ulid proposerId, VideoGame videoGame)
    {
        Id = Ulid.NewUlid();
        FriendGroupId = friendGroupId;
        ProposerId = proposerId;
        StartDateTimeUtc = DateTimeOffset.UtcNow;
        VideoGame = videoGame;
    }
    
    public Ulid Id { get; private init; }
    public Ulid FriendGroupId { get; private init; }
    public DateTimeOffset StartDateTimeUtc { get; private init; }
    public Ulid ProposerId { get; private init; }
    public VideoGame VideoGame { get; private init; }
    
    public Ulid? PlayerId { get; private set; }
    public Ulid? VetoerId { get; private set; }
    public bool? Vetoed { get; private set; }
    public DateTimeOffset? EndDateTimeUtc { get; private set; }

    internal static GameProposal StartProposal(FriendGroup friendGroup, Friend proposer, VideoGame videoGame)
    {
        return new GameProposal(friendGroup.Id, proposer.Id, videoGame);
    }
    
    internal void EndSuccessfulProposal(Friend player)
    {
        PlayerId = player.Id;
        Vetoed = false;
        EndDateTimeUtc = DateTimeOffset.UtcNow;
    }
    
    internal void EndFailedProposal(Friend vetoer)
    {
        VetoerId = vetoer.Id;
        Vetoed = true;
        EndDateTimeUtc = DateTimeOffset.UtcNow;
    }
}