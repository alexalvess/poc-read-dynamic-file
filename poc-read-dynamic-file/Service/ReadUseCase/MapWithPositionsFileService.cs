using MassTransit;
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
    private readonly IPublishEndpoint _publishEndpoint;

    public MapWithPositionsFileService(
        IOptions<ColumnFileOption<PositionFieldOption>> columnFileOption, 
        IOptions<FilePathOption> filePathOption,
        ReadFileService service,
        UserRepository repository,
        IPublishEndpoint publishEndpoint)
    {
        _columnFileOption = columnFileOption.Value;
        _filePathOption = filePathOption.Value;
        _service = service;
        _repository = repository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task PositionsWithStreamReaderAsync(bool sendToMessageBus = false)
    {
        using FileStream stream = new(_filePathOption.PositionFilePath, FileMode.Open, FileAccess.Read);
        List<UserModel> users = new();

        await _service.StreamReaderAsync(
            stream: stream,
            proccessLine: line =>
            {
                UserModel user = new(line, _columnFileOption);

                if(sendToMessageBus)
                {
                    users.Add(user);
                    return Task.CompletedTask;
                }

                return _repository.UpsertAsync(user);
            },
            cancellationToken: default);

        await Task.WhenAll(users.Select(user => _publishEndpoint.Publish(user, typeof(UserModel))));
    }

    public async Task PositionsWithPipeReaderAsync(bool sendToMessageBus = false)
    {
        using FileStream stream = new(_filePathOption.PositionFilePath, FileMode.Open, FileAccess.Read);
        List<UserModel> users = new();

        await _service.PipeReaderAsync(
            stream: stream,
            proccessLine: line =>
            {
                UserModel user = new(line, _columnFileOption);
                
                if (sendToMessageBus)
                {
                    users.Add(user);
                    return Task.CompletedTask;
                }

                return _repository.UpsertAsync(user);
            },
            cancellationToken: default);

        await Task.WhenAll(users.Select(user => _publishEndpoint.Publish(user, typeof(UserModel))));
    }
}
