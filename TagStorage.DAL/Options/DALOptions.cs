namespace TagStorage.DAL.Options;

public record DALOptions
{
    public required string DatabaseFilePath { get; init; }
    public bool RecreateDatabaseEachTime { get; init; } = false;
}
