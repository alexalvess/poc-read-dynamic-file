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
    public async Task<IEnumerable<UserModel>> RecoverDataWithPipelineAsync()
    {
        var command = new CommandDefinition(@"SELECT * FROM [pocFile].[dbo].[User]", flags: CommandFlags.Pipelined);
        return await _dbContext.Connection.QueryAsync<UserModel>(command);
    }

    [Benchmark]
    public async Task<IEnumerable<UserModel>> RecoverDataAsync()
        => await _dbContext.Connection.QueryAsync<UserModel>(
            sql: @"SELECT * FROM [pocFile].[dbo].[User]", new[] { typeof(UserModel) }, default, buffered: false);

    [Benchmark]
    public IEnumerable<UserModel> RecoverData()
        => _dbContext.Connection.Query<UserModel>(
            sql: @"SELECT * FROM [pocFile].[dbo].[User]", buffered: false);

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
