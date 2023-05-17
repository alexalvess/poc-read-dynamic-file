namespace poc_read_dynamic_file.Options;

public record FilePathOption
{
    public string PositionFilePath { get; init; }

    public string SeparatorFilePath { get; init; }
}
