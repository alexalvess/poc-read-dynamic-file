using System.Data;
using System.Data.SqlClient;

namespace poc_read_dynamic_file.Service.WriteUseCase.Database;

public class DbContext : IDisposable
{
    private const string connectionString = "Server=127.0.0.1;User Id=sa;Password=myPassword!;";

    public DbContext()
    {
        Connection = new SqlConnection(connectionString);
        Connection.Open();
    }

    public IDbConnection Connection { get; }

    public void Dispose()
    {
        if(Connection.State != ConnectionState.Closed)
            Connection.Close();

        Connection.Dispose();
    }
}