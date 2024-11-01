namespace InMa.GameNight;

public record VideoGame
{
    // will use https://www.giantbomb.com/api/documentation/ for the game details, to integrate some lookup automation
    // will also use this api's id format for uniqueness
    // if the game is not found the id will be generated
    
    private VideoGame(string guid, int id, string name, DateTimeOffset? releaseDate, string summary)
    {
        Custom = false;
        
        Guid = guid;
        Id = id;
        Name = name;
        ReleaseDate = releaseDate;
        Summary = summary;
    }

    private VideoGame(string name)
    {
        Custom = true;
        
        Guid = Ulid.NewUlid().ToString();
        Name = name;
    }
    
    public string Guid { get; private init; }
    public bool Custom { get; }
    
    public string Name { get; } 
    
    public int? Id { get; }
    public DateTimeOffset? ReleaseDate { get; }
    public string? Summary { get; }

    public static VideoGame Create(VideoGameDto videoGameDto)
    {
        DateTimeOffset.TryParseExact(videoGameDto.original_release_date, "yyyy-MM-dd", null,
            System.Globalization.DateTimeStyles.None, out var releaseDate);

        return new(videoGameDto.guid, videoGameDto.id, videoGameDto.name, releaseDate, videoGameDto.deck);
    }

    public static VideoGame CreateCustom(string name) => new(name);
}