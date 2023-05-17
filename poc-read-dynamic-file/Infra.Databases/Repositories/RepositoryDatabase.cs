using BenchmarkDotNet.Attributes;
using Dapper;
using poc_read_dynamic_file.Infra.Databases.Contexts;
using poc_read_dynamic_file.Models;
using System.Data;

namespace poc_read_dynamic_file.Infra.Databases.Repositories;

public class RepositoryDatabase : IDisposable
{
    private readonly DbContext _dbContext;

    public RepositoryDatabase(DbContext dbContext)
        => _dbContext = dbContext;

    public Task UpsertAsync()
    {
        var sql = @"
            INSERT INTO User (
                [Name]
				,[Email]
				,[ProductCode]
				,[PaymentDate]
				,[PaymentValue])
            VALUES(
                @Name
				,@Email
				,@ProductCode
				,@PaymentDate
				,@PaymentValue) 
            ON CONFLICT ON CONSTRAINT users_id_key
            DO NOTHING;";
    }

    public IEnumerable<UserModel> Users { get; private set; }

    [Benchmark(Description = "Async way and Pipelined Flag")]
    public async Task RecoverDataWithPipelineAsync()
    {
        var command = new CommandDefinition(@"SELECT * FROM [pocFile].[dbo].[User]", flags: CommandFlags.Pipelined);
        Users = await _dbContext.Connection.QueryAsync<UserModel>(command);
    }

    [Benchmark(Description = "Async way and none Flag")]
    public async Task RecoverDataAsync()
    {
        var command = new CommandDefinition(@"SELECT * FROM [pocFile].[dbo].[User]", flags: CommandFlags.None);
        Users = await _dbContext.Connection.QueryAsync<UserModel>(command);
    }

    [Benchmark(Description = "Sync way and none buffered")]
    public void RecoverData()
        => Users = _dbContext.Connection.Query<UserModel>(
            sql: @"SELECT * FROM [pocFile].[dbo].[User]", buffered: false)
        .Select(s => s);

    [Benchmark(Description = "Sync way and buffered")]
    public void RecoverDataWithBuffered()
        => Users = _dbContext.Connection.Query<UserModel>(
            sql: @"SELECT * FROM [pocFile].[dbo].[User]")
        .Select(s => s);

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
