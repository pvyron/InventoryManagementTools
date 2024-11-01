namespace InMa.GameNight;

public sealed class Friend
{
    private Friend(string nickname)
    {
        Id = Ulid.NewUlid();
        _nickname = nickname;
    }

    private Friend(string firstName, string lastName)
    {
        Id = Ulid.NewUlid();
        FirstName = firstName;
        LastName = lastName;
    }

    public Ulid Id { get; private init; }

    private string? _nickname;
    public string NickName => _nickname ?? FirstName!;
    
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }

    public static Friend Create(string firstName, string lastName) => new(firstName, lastName);
    public static Friend Create(string nickname) => new(nickname);

    public void ChangeNickname(string newNickname)
    {
        _nickname = newNickname;
    }

    public void ChangeFirstName(string newFirstName)
    {
        FirstName = newFirstName;
    }

    public void ChangeLastName(string newLastName)
    {
        LastName = newLastName;
    }
}