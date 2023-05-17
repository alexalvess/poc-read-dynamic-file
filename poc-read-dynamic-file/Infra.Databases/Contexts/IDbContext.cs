using System.Data;

namespace poc_read_dynamic_file.Infra.Databases.Contexts;

public interface IDbContext : IDisposable
{
    IDbConnection Connection { get; }
}
