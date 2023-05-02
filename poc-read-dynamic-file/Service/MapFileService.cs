using Microsoft.Extensions.Options;
using poc_read_dynamic_file.Extensions;
using poc_read_dynamic_file.Models;
using poc_read_dynamic_file.Options;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace poc_read_dynamic_file.Service;

public class MapFileService
{
    private readonly ColumnFileOption<string> _separatorOptions;
    private readonly ColumnFileOption<PositionFieldOption> _positionOptions;
    private readonly ReadFileService _readFileService;

    public MapFileService(
        ReadFileService readFileService,
        IOptions<ColumnFileOption<string>> separatorOptions,
        IOptions<ColumnFileOption<PositionFieldOption>> positionOptions)
    {
        _readFileService = readFileService;
        _separatorOptions = separatorOptions.Value;
        _positionOptions = positionOptions.Value;
    }

    public async Task MapSeparatorFileAsync(CancellationToken cancellationToken)
    {
        const string path = "..\\..\\..\\..\\.assets\\sample-separator-file.txt";

        await MapWithStreamReaderAsync(path, cancellationToken);
        await MapWithPipeReaderAsync(path, cancellationToken);
    }

    public async Task MapPositionsFileAsync(CancellationToken cancellationToken)
    {
        const string path = "..\\..\\..\\..\\.assets\\sample-positions-file.txt";
        using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        using var reader = new StreamReader(fileStream, Encoding.UTF8);

        List<UserModel> users = new();

        while (true)
        {
            var line = await reader.ReadLineAsync(cancellationToken);

            if (line == null)
                break;

            UserModel user = new UserModel(line, _positionOptions);
            users.Add(user);
        }
    }

    private async Task MapWithStreamReaderAsync(string path, CancellationToken cancellationToken)
    {
        using FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
        List<UserModel> users = new();
        IDictionary<string, int>? headers = null;

        var sw = new Stopwatch();
        var before2 = GC.CollectionCount(2);
        var before1 = GC.CollectionCount(1);
        var before0 = GC.CollectionCount(0);
        sw.Start();

        await _readFileService.StreamReaderAsync(
            stream: stream,
            proccessLine: line =>
            {
                if (headers is null)
                {
                    headers = line.MapFields(_separatorOptions, '|');
                    return;
                }

                UserModel user = new(line.Split('|'), headers, _separatorOptions);

                users.Add(user);
            },
            cancellationToken: cancellationToken);

        sw.Stop();

        var gen2 = GC.CollectionCount(2) - before2;
        var gen1 = GC.CollectionCount(1) - before1;
        var gen0 = GC.CollectionCount(0) - before0;

        Console.WriteLine($"Total time: {sw.ElapsedMilliseconds} ms");
        Console.WriteLine($"GC Gen #2: {GC.CollectionCount(2) - before2}");
        Console.WriteLine($"GC Gen #1: {GC.CollectionCount(1) - before1}");
        Console.WriteLine($"GC Gen #0: {GC.CollectionCount(0) - before0}\n");

        GC.Collect();
    }

    private async Task MapWithPipeReaderAsync(string path, CancellationToken cancellationToken)
    {
        using FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
        List<UserModel> users = new();
        IDictionary<string, int>? headers = null;

        var sw = new Stopwatch();
        var before2 = GC.CollectionCount(2);
        var before1 = GC.CollectionCount(1);
        var before0 = GC.CollectionCount(0);
        sw.Start();

        await _readFileService.PipeReaderAsync(
            stream: stream,
            proccessLine: line =>
            {
                if (headers is null)
                {
                    headers = line.MapFields(_separatorOptions, '|');
                    return;
                }

                UserModel user = new(line.Split('|'), headers, _separatorOptions);

                users.Add(user);
            },
            cancellationToken: cancellationToken);

        sw.Stop();

        var gen2 = GC.CollectionCount(2) - before2;
        var gen1 = GC.CollectionCount(1) - before1;
        var gen0 = GC.CollectionCount(0) - before0;

        Console.WriteLine($"Total time: {sw.ElapsedMilliseconds} ms");
        Console.WriteLine($"GC Gen #2: {GC.CollectionCount(2) - before2}");
        Console.WriteLine($"GC Gen #1: {GC.CollectionCount(1) - before1}");
        Console.WriteLine($"GC Gen #0: {GC.CollectionCount(0) - before0}\n");

        GC.Collect();
    }
}