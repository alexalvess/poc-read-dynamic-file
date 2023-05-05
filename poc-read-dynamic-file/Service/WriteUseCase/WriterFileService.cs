using BenchmarkDotNet.Attributes;
using Dapper;
using poc_read_dynamic_file.Models;
using poc_read_dynamic_file.Service.WriteUseCase.Database;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;

namespace poc_read_dynamic_file.Service.WriteUseCase;

[MemoryDiagnoser]
public class WriterFileService : IDisposable
{
    private const string path = "C:\\src\\github\\poc-read-dynamic-file\\.assets\\sample-writer-file-{0}-new.txt";

    private readonly RepositoryDatabase _repository;

    public WriterFileService()
    {
        _repository = new RepositoryDatabase();
    }

    [Benchmark]
    public async Task WithStreamAsync()
    {
        if (File.Exists(string.Format(path, "stream")))
            File.Delete(string.Format(path, "stream"));

        using StreamWriter streamWriter = new(string.Format(path, "stream"), options: new()
        {
            Access = FileAccess.Write,
            Mode = FileMode.OpenOrCreate,
        });

        foreach (var user in await RecoverDataWithPipelineAsync())
            await streamWriter.WriteLineAsync(JsonSerializer.Serialize(user));
    }

    [Benchmark]
    public async Task WithPipelineWriteAsync()
    {
        const int minimumBufferSize = 512;

        if (File.Exists(string.Format(path, "pipeline-write")))
            File.Delete(string.Format(path, "pipeline-write"));

        using FileStream stream = new(string.Format(path, "pipeline-write"), FileMode.OpenOrCreate, FileAccess.Write);
        var writer = PipeWriter.Create(stream);

        foreach (var user in await RecoverDataWithPipelineAsync())
        {
            Memory<byte> memory = writer.GetMemory(minimumBufferSize);
            try
            {
                await writer.WriteAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(user)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                break;
            }
        }

        await writer.CompleteAsync();
    }

    [Benchmark]
    public async Task WithPipelineAdvanceAsync()
    {
        const int minimumBufferSize = 512;

        if (File.Exists(string.Format(path, "pipeline-advance")))
            File.Delete(string.Format(path, "pipeline-advance"));

        using FileStream stream = new(string.Format(path, "pipeline-advance"), FileMode.OpenOrCreate, FileAccess.Write);
        var writer = PipeWriter.Create(stream);

        foreach (var user in await RecoverDataWithPipelineAsync())
        {
            Memory<byte> memory = writer.GetMemory(minimumBufferSize);
            try
            {
                writer.Advance(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(user), memory.Span));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                break;
            }

            await writer.FlushAsync();
        }

        await writer.CompleteAsync();
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
