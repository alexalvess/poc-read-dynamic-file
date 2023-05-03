using poc_read_dynamic_file.Extensions;
using poc_read_dynamic_file.Models;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Text;

namespace poc_read_dynamic_file.Service.ReadUseCase;

public class ReadFileService
{
    public async Task StreamReaderAsync(Stream stream, Action<string> proccessLine, CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8);

        while (true)
        {
            var line = await reader.ReadLineAsync(cancellationToken) ?? null;

            if (line is null)
                break;

            proccessLine(line);
        }
    }

    public async Task PipeReaderAsync(Stream stream, Action<string> proccessLine, CancellationToken cancellationToken)
    {
        PipeReader reader = PipeReader.Create(stream);

        while (true)
        {
            ReadResult result = await reader.ReadAsync();
            ReadOnlySequence<byte> buffer = result.Buffer;

            if (TryReadLine(ref buffer, out ReadOnlySequence<byte> line))
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
