namespace InMa.GameNight.Tests;

public class GameProposalUnitTests
{
    private FriendGroup DummyGroup()
    {
        var friendGroup = 
            FriendGroup.Create(Ulid.NewUlid(), "Test Friend Group", "A friend group to test");

        friendGroup.AddFriend(Friend.Create("tester1"));
        friendGroup.AddFriend(Friend.Create("tester2"));
        
        return friendGroup;
    }

    private VideoGame DummyCustomVideoGame => VideoGame.CreateCustom("Test VideoGame");
    
    [Test]
    public async Task GameProposalStartTest_Should_Populate_ActiveProposal()
    {
        // Arrange
        var friendGroup = DummyGroup();
        var videoGame = DummyCustomVideoGame;

        var proposer = friendGroup.Friends.FirstOrDefault(f => f.NickName == "tester1")!;
        
        // Act
        var proposalStarted = friendGroup.StartProposal(proposer, videoGame);

        // Assert
        await Assert.That(proposalStarted).IsTrue();
        await Assert.That(friendGroup.ActiveProposal).IsNotNull();
        await Assert.That(friendGroup.PastGameProposals.Count).IsEqualTo(0);
    }
    
    [Test]
    public async Task GameProposalEndTest_Should_Depopulate_ActiveProposal_And_Add_To_PastProposals()
    {
        // Arrange
        var friendGroup = DummyGroup();
        var videoGame = DummyCustomVideoGame;

        var proposer = friendGroup.Friends.FirstOrDefault(f => f.NickName == "tester1")!;
        
        // Act
        var proposalStarted = friendGroup.StartProposal(proposer, videoGame);
        var proposalEnded = friendGroup.EndProposal(proposer);
        
        // Assert
        await Assert.That(proposalStarted).IsTrue();
        await Assert.That(proposalEnded).IsTrue();
        await Assert.That(friendGroup.ActiveProposal).IsNull();
        await Assert.That(friendGroup.PastGameProposals.Count).IsEqualTo(1);
        await Assert.That(friendGroup.PastGameProposals.First()).IsNotNull();
    }
    
    [Test]
    public async Task GameProposalEndTest_Should_Add_Veto_To_Player()
    {
        // Arrange
        var friendGroup = DummyGroup();
        var videoGame = DummyCustomVideoGame;

        var proposer = friendGroup.Friends.FirstOrDefault(f => f.NickName == "tester1")!;
        var player = friendGroup.Friends.FirstOrDefault(f => f.NickName == "tester2")!;
        var startingPlayerVetoes = friendGroup.VetoesPerFriend[player.Id];
        
        // Act
        var proposalStarted = friendGroup.StartProposal(proposer, videoGame);
        var proposalEnded = friendGroup.EndProposal(player);
        
        // Assert
        await Assert.That(proposalStarted).IsTrue();
        await Assert.That(proposalEnded).IsTrue();
        await Assert.That(friendGroup.VetoesPerFriend[player.Id]).IsEqualTo((ushort)(startingPlayerVetoes + 1));
    }
    
    [Test]
    public async Task GameProposalEndTest_Should_Fail_With_No_Active_Proposal()
    {
        // Arrange
        var friendGroup = DummyGroup();

        var player = friendGroup.Friends.FirstOrDefault(f => f.NickName == "tester1")!;
        
        // Act
        var proposalEnded = friendGroup.EndProposal(player);
        
        // Assert
        await Assert.That(proposalEnded).IsFalse();
        await Assert.That(friendGroup.ActiveProposal).IsNull();
        await Assert.That(friendGroup.PastGameProposals.Count).IsEqualTo(0);
    }
    
    [Test]
    public async Task GameProposalVetoTest_Should_Substruct_Veto_From_Player()
    {
        // Arrange
        var friendGroup = DummyGroup();
        var videoGame = DummyCustomVideoGame;

        var proposer = friendGroup.Friends.FirstOrDefault(f => f.NickName == "tester1")!;
        var vetoer = friendGroup.Friends.FirstOrDefault(f => f.NickName == "tester2")!;
        var startingVetoerVetoes = friendGroup.VetoesPerFriend[vetoer.Id];
        
        // Act
        var proposalStarted = friendGroup.StartProposal(proposer, videoGame);
        var proposalEnded = friendGroup.VetoProposal(vetoer);
        
        // Assert
        await Assert.That(proposalStarted).IsTrue();
        await Assert.That(proposalEnded).IsTrue();
        await Assert.That(friendGroup.VetoesPerFriend[vetoer.Id]).IsEqualTo((ushort)(startingVetoerVetoes - 1));
    }
    
    [Test]
    public async Task GameProposalVetoTest_Should_Fail_With_No_Veto_From_Player()
    {
        // Arrange
        var friendGroup = DummyGroup();
        var videoGame = DummyCustomVideoGame;

        var proposer = friendGroup.Friends.FirstOrDefault(f => f.NickName == "tester1")!;
        var vetoer = friendGroup.Friends.FirstOrDefault(f => f.NickName == "tester2")!;
        friendGroup.UpdateFriendsVetoes(vetoer, 0);
        
        // Act
        var proposalStarted = friendGroup.StartProposal(proposer, videoGame);
        var proposalEnded = friendGroup.VetoProposal(vetoer);
        
        // Assert
        await Assert.That(proposalStarted).IsTrue();
        await Assert.That(proposalEnded).IsFalse();
        await Assert.That(friendGroup.ActiveProposal).IsNotNull();
        await Assert.That(friendGroup.PastGameProposals.Count).IsEqualTo(0);
    }
}