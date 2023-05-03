using BenchmarkDotNet.Attributes;
using poc_read_dynamic_file.Extensions;
using poc_read_dynamic_file.Models;
using poc_read_dynamic_file.Options;

namespace poc_read_dynamic_file.Service.ReadUseCase;

[MemoryDiagnoser]
public class MapWithPositionsFileService
{
    private readonly ColumnFileOption<PositionFieldOption> _options;
    private readonly ReadFileService _service;

    private const string path = "C:\\src\\github\\poc-read-dynamic-file\\.assets\\sample-positions-file.txt";

    public MapWithPositionsFileService()
    {
        _service = new ReadFileService();

        _options = new()
        {
            Name = new()
            {
                Start = 0,
                Length = 15
            },
            Email = new()
            {
                Start = 15,
                Length = 24
            },
            ProductCode = new()
            {
                Start = 39,
                Length = 10
            },
            PaymentDate = new()
            {
                Start = 49,
                Length = 11
            },
            PaymentValue = new()
            {
                Start = 60,
                Length = 5
            }
        };
    }

    [Benchmark]
    public async Task PositionsWithStreamReaderAsync()
    {
        using FileStream stream = new(path, FileMode.Open, FileAccess.Read);
        List<UserModel> users = new();

        await _service.StreamReaderAsync(
            stream: stream,
            proccessLine: line =>
            {
                UserModel user = new(line, _options);
                users.Add(user);
            },
            cancellationToken: default);
    }

    [Benchmark]
    public async Task PositionsWithPipeReaderAsync()
    {
        using FileStream stream = new(path, FileMode.Open, FileAccess.Read);
        List<UserModel> users = new();

        await _service.PipeReaderAsync(
            stream: stream,
            proccessLine: line =>
            {
                UserModel user = new(line, _options);
                users.Add(user);
            },
            cancellationToken: default);
    }
}
