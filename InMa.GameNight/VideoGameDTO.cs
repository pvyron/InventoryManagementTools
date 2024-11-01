namespace InMa.GameNight;

public record VideoGameDto(
    string aliases,
    string deck,
    string guid,
    int id,
    string name,
    string original_release_date
);