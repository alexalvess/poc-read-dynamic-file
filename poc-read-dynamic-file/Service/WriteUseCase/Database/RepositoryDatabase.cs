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

    public Task<IEnumerable<UserModel>> RecoverDataAsync()
        => _dbContext.Connection.QueryAsync<UserModel>(@"SELECT * FROM [pocFile].[dbo].[User]", buffered: false);

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
