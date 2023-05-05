using BenchmarkDotNet.Attributes;
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

    [Benchmark]
    public async Task RecoverDataWithPipelineAsync()
    {
        var command = new CommandDefinition(@"SELECT * FROM [pocFile].[dbo].[User]", flags: CommandFlags.Pipelined);
        var temp = await _dbContext.Connection.QueryAsync<UserModel>(command);
    }

    [Benchmark]
    public async Task RecoverDataAsync()
    {
        var command = new CommandDefinition(@"SELECT * FROM [pocFile].[dbo].[User]", flags: CommandFlags.None);
        var temp = await _dbContext.Connection.QueryAsync<UserModel>(command);
    }

    [Benchmark]
    public void RecoverData()
        => _dbContext.Connection.Query<UserModel>(
            sql: @"SELECT * FROM [pocFile].[dbo].[User]", buffered: false).Select(s => s);

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
