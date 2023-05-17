using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace poc_read_dynamic_file.Infra.Databases.Contexts;

public class DbContext : IDbContext
{
    public DbContext(IConfiguration configuration)
    {
        Connection = new SqlConnection(configuration.GetConnectionString("Postgres"));
        Connection.Open();
    }

    public IDbConnection Connection { get; }

    public void Dispose()
    {
        if (Connection.State != ConnectionState.Closed)
            Connection.Close();

        Connection.Dispose();
    }
}