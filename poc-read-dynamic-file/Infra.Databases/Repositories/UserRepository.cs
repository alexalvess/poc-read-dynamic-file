using BenchmarkDotNet.Attributes;
using Dapper;
using poc_read_dynamic_file.Infra.Databases.Contexts;
using poc_read_dynamic_file.Models;
using System.Data;

namespace poc_read_dynamic_file.Infra.Databases.Repositories;

public class UserRepository
{
    private readonly IDbContext _dbContext;

    public UserRepository(IDbContext dbContext)
        => _dbContext = dbContext;

    public Task UpsertAsync(UserModel user)
    {
        var sql = @"
            INSERT INTO users (
                name
				,email
				,productcode
				,paymentdate
				,paymentvalue)
            VALUES(
                @Name
				,@Email
				,@ProductCode
				,@PaymentDate
				,@PaymentValue) 
            ON CONFLICT ON CONSTRAINT users_pkey
            DO NOTHING;";

        var command = new CommandDefinition(sql, new
        {
            user.Name,
            user.Email,
            user.ProductCode,
            user.PaymentDate,
            user.PaymentValue
        });

        return _dbContext.Connection.ExecuteAsync(command);
    }
}
