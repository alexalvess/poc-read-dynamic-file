using System.Buffers;
using System.IO.Pipelines;
using System.Text;

namespace poc_read_dynamic_file.Service.ReadUseCase;

public class ReadFileService
{
    public async Task StreamReaderAsync(Stream stream, Func<string, Task> proccessLine, CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8);

        while (true)
        {
            var line = await reader.ReadLineAsync(cancellationToken) ?? null;

            if (string.IsNullOrWhiteSpace(line))
                break;

            await proccessLine(line);
        }
    }

    public async Task PipeReaderAsync(Stream stream, Func<string, Task> proccessLine, CancellationToken cancellationToken)
    {
        PipeReader reader = PipeReader.Create(stream);
        bool lastLine = false;

        while (true)
        {
            ReadResult result = await reader.ReadAsync();
            ReadOnlySequence<byte> buffer = result.Buffer;

            if (TryReadLine(ref buffer, ref lastLine, out string line))
                await proccessLine(line);

            reader.AdvanceTo(buffer.Start, buffer.End);

            if (string.IsNullOrWhiteSpace(line))
                break;
        }

        await reader.CompleteAsync();
    }

    private static bool TryReadLine(ref ReadOnlySequence<byte> buffer, ref bool isComplete, out string line)
    {
        SequencePosition? position = buffer.PositionOf((byte)'\n');

        if (position == null)
        {
            if(isComplete is false)
            {
                position = buffer.End;
                isComplete = true;
            }
            else
            {
                line = string.Empty;
                return false;
            }
        }

        line = Encoding.UTF8.GetString(buffer.Slice(0, position.Value));

        if(isComplete is false)
            buffer = buffer.Slice(buffer.GetPosition(1, position.Value));

        return true && string.IsNullOrWhiteSpace(line) is false;
    }
}
