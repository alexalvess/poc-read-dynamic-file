namespace poc_read_dynamic_file.Service.WriteUseCase.Database;

public class RepositoryDatabase : IDisposable
{
    private readonly DbContext _dbContext;

    public RepositoryDatabase()
    {
        _dbContext= new DbContext();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
