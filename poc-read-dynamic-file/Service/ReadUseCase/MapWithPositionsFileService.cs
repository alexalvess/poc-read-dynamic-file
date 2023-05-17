using Microsoft.Extensions.Options;
using poc_read_dynamic_file.Models;
using poc_read_dynamic_file.Options;

namespace poc_read_dynamic_file.Service.ReadUseCase;

public class MapWithPositionsFileService
{
    private readonly ColumnFileOption<PositionFieldOption> _columnFileOption;
    private readonly FilePathOption _filePathOption;
    private readonly ReadFileService _service;

    public MapWithPositionsFileService(
        IOptions<ColumnFileOption<PositionFieldOption>> columnFileOption, 
        IOptions<FilePathOption> filePathOption,
        ReadFileService service)
    {
        _columnFileOption = columnFileOption.Value;
        _filePathOption = filePathOption.Value;
        _service = service;
    }

    public async Task PositionsWithStreamReaderAsync()
    {
        using FileStream stream = new(_filePathOption.SeparatorFilePath, FileMode.Open, FileAccess.Read);
        List<UserModel> users = new();

        await _service.StreamReaderAsync(
            stream: stream,
            proccessLine: line =>
            {
                UserModel user = new(line, _columnFileOption);
                users.Add(user);
            },
            cancellationToken: default);
    }

    public async Task PositionsWithPipeReaderAsync()
    {
        using FileStream stream = new(_filePathOption.SeparatorFilePath, FileMode.Open, FileAccess.Read);
        List<UserModel> users = new();

        await _service.PipeReaderAsync(
            stream: stream,
            proccessLine: line =>
            {
                UserModel user = new(line, _columnFileOption);
                users.Add(user);
            },
            cancellationToken: default);
    }
}
