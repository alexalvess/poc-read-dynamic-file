using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Dapper;
using poc_read_dynamic_file.Models;
using System.Data;

namespace poc_read_dynamic_file.Service.WriteUseCase.Database;

[MemoryDiagnoser]
public class RepositoryDatabase : IDisposable
{
    private readonly DbContext _dbContext;

    public RepositoryDatabase()
    {
        _dbContext= new DbContext();
    }

    public IEnumerable<UserModel> Users { get; private set; }

    [Benchmark(Description = "Recover data with async way and Pipelined Flag")]
    public async Task RecoverDataWithPipelineAsync()
    {
        var command = new CommandDefinition(@"SELECT * FROM [pocFile].[dbo].[User]", flags: CommandFlags.Pipelined);
        Users = await _dbContext.Connection.QueryAsync<UserModel>(command);
    }

    [Benchmark(Description = "Recover data with async way and none Flag")]
    public async Task RecoverDataAsync()
    {
        var command = new CommandDefinition(@"SELECT * FROM [pocFile].[dbo].[User]", flags: CommandFlags.None);
        Users = await _dbContext.Connection.QueryAsync<UserModel>(command);
    }

    [Benchmark(Description = "Recover data with sync way and none buffered")]
    public void RecoverData()
        => Users = _dbContext.Connection.Query<UserModel>(
            sql: @"SELECT * FROM [pocFile].[dbo].[User]", buffered: false)
        .Select(s => s);

    [Benchmark(Description = "Recover data with sync way and buffered")]
    public void RecoverDataWithBuffered()
        => Users = _dbContext.Connection.Query<UserModel>(
            sql: @"SELECT * FROM [pocFile].[dbo].[User]")
        .Select(s => s);

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
