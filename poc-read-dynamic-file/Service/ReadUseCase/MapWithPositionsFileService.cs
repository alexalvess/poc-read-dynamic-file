using Microsoft.Extensions.Options;
using poc_read_dynamic_file.Infra.Databases.Repositories;
using poc_read_dynamic_file.Models;
using poc_read_dynamic_file.Options;

namespace poc_read_dynamic_file.Service.ReadUseCase;

public class MapWithPositionsFileService
{
    private readonly ColumnFileOption<PositionFieldOption> _columnFileOption;
    private readonly FilePathOption _filePathOption;
    private readonly ReadFileService _service;
    private readonly UserRepository _repository;

    public MapWithPositionsFileService(
        IOptions<ColumnFileOption<PositionFieldOption>> columnFileOption, 
        IOptions<FilePathOption> filePathOption,
        ReadFileService service,
        UserRepository repository)
    {
        _columnFileOption = columnFileOption.Value;
        _filePathOption = filePathOption.Value;
        _service = service;
        _repository = repository;
    }

    public async Task PositionsWithStreamReaderAsync()
    {
        using FileStream stream = new(_filePathOption.PositionFilePath, FileMode.Open, FileAccess.Read);

        await _service.StreamReaderAsync(
            stream: stream,
            proccessLine: line =>
            {
                UserModel user = new(line, _columnFileOption);
                return Task.CompletedTask;
            },
            cancellationToken: default);
    }

    public async Task PositionsWithPipeReaderAsync()
    {
        using FileStream stream = new(_filePathOption.PositionFilePath, FileMode.Open, FileAccess.Read);

        await _service.PipeReaderAsync(
            stream: stream,
            proccessLine: line =>
            {
                UserModel user = new(line, _columnFileOption);
                return Task.CompletedTask;
            },
            cancellationToken: default);
    }
}
