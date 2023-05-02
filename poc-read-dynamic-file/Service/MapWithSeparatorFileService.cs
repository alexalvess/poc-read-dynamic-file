using BenchmarkDotNet.Attributes;
using poc_read_dynamic_file.Extensions;
using poc_read_dynamic_file.Models;
using poc_read_dynamic_file.Options;

namespace poc_read_dynamic_file.Service;

[MemoryDiagnoser]
public class MapWithSeparatorFileService
{
    private readonly ColumnFileOption<string> _options;
    private readonly ReadFileService _service;

    private const string path = "C:\\src\\github\\poc-read-dynamic-file\\.assets\\sample-separator-file.txt";

    public MapWithSeparatorFileService()
    {
        _service = new ReadFileService();

        _options = new()
        {
            Name = "USER_NAME",
            Email =  "EMAIL",
            ProductCode =  "PRD_CODE",
            PaymentDate =  "PAY_DATE",
            PaymentValue =  "PAY_VALUE"
        };
    }

    [Benchmark]
    public async Task SeparatorWithStreamReaderAsync()
    {
        using FileStream stream = new(path, FileMode.Open, FileAccess.Read);
        List<UserModel> users = new();
        IDictionary<string, int>? headers = null;

        await _service.StreamReaderAsync(
            stream: stream,
            proccessLine: line =>
            {
                if (headers is null)
                {
                    headers = line.MapFields(_options, '|');
                    return;
                }

                UserModel user = new(line.Split('|'), headers, _options);
                users.Add(user);
            },
            cancellationToken: default);
    }

    [Benchmark]
    public async Task SeparatorWithPipeReaderAsync()
    {
        using FileStream stream = new(path, FileMode.Open, FileAccess.Read);
        List<UserModel> users = new();
        IDictionary<string, int>? headers = null;

        await _service.PipeReaderAsync(
            stream: stream,
            proccessLine: line =>
            {
                if (headers is null)
                {
                    headers = line.MapFields(_options, '|');
                    return;
                }

                UserModel user = new(line.Split('|'), headers, _options);
                users.Add(user);
            },
            cancellationToken: default);
    }
}