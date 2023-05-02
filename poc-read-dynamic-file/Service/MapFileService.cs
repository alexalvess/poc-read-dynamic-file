using Microsoft.Extensions.Options;
using poc_read_dynamic_file.Extensions;
using poc_read_dynamic_file.Models;
using poc_read_dynamic_file.Options;
using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Text;

namespace poc_read_dynamic_file.Service;

public class MapFileService
{
    private readonly ColumnFileOption<string> _separatorOptions;
    private readonly ColumnFileOption<PositionFieldOption> _positionOptions;

    public MapFileService(
        IOptions<ColumnFileOption<string>> separatorOptions,
        IOptions<ColumnFileOption<PositionFieldOption>> positionOptions)
    {
        _separatorOptions = separatorOptions.Value;
        _positionOptions = positionOptions.Value;
    }

    public async Task MapSeparatorFileAsync(CancellationToken cancellationToken)
    {
        const string path = "..\\..\\..\\..\\.assets\\sample-separator-file.txt";
        using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);

        await StreamPipelineAsync(fileStream);

        using var reader = new StreamReader(fileStream, Encoding.UTF8);

        string headerLine = await reader.ReadLineAsync(cancellationToken) ?? String.Empty;
        var headers = headerLine.MapFields(_separatorOptions, '|');

        List<UserModel> users = new();

        while (true)
        {
            var line = await reader.ReadLineAsync(cancellationToken);

            if (line == null) 
                break;

            UserModel user = new UserModel(line.Split('|'), headers, _separatorOptions);
            users.Add(user);
        }
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

    private static async Task PipeReaderAsync(Stream stream, Action<string> proccessLine,  CancellationToken cancellationToken)
    {
        PipeReader reader = PipeReader.Create(stream);

        while (true)
        {
            ReadResult result = await reader.ReadAsync();
            ReadOnlySequence<byte> buffer = result.Buffer;

            if(TryReadLine(ref buffer, out ReadOnlySequence<byte> line))
                proccessLine(Encoding.UTF8.GetString(line));

            reader.AdvanceTo(buffer.Start, buffer.End);

            if (result.IsCompleted)
                break;
        }

        await reader.CompleteAsync();
    }

    private static bool TryReadLine(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> line)
    {
        SequencePosition? position = buffer.PositionOf((byte)'\n');

        if (position == null)
        {
            line = default;
            return false;
        }

        line = buffer.Slice(0, position.Value);
        buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
        return true;
    }
}