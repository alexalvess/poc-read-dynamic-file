using Dapper;
using poc_read_dynamic_file.Models;

namespace poc_read_dynamic_file.Service.WriteUseCase.Database;

public class RepositoryDatabase : IDisposable
{
    private readonly DbContext _dbContext;

    public RepositoryDatabase()
    {
        _dbContext= new DbContext();
    }

    public Task<IEnumerable<UserModel>> RecoverData()
        => _dbContext.Connection.QueryAsync<UserModel>(
            sql: @"SELECT * FROM [pocFile].[dbo].[User]", new[] { typeof(UserModel) }, default, buffered: false);

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
