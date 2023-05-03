using Bogus;
using Bogus.DataSets;
using Dapper;
using poc_read_dynamic_file.Models;

namespace poc_read_dynamic_file.Service.WriteUseCase.Database;

public class Seed : IDisposable
{
    private readonly DbContext _dbContext;

	public Seed()
	{
		_dbContext= new DbContext();
	}

	public async Task DataSeedAsync()
	{
		await CreateDatabaseAsync();
		await CreateTablesAsync();
		await InsertDatasAsync();
	}

	private Task CreateDatabaseAsync()
		=> _dbContext.Connection.ExecuteAsync(@"
	IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'pocFile')
		BEGIN
			CREATE DATABASE [pocFile]
		END;");

	private Task CreateTablesAsync()
		=> _dbContext.Connection.ExecuteAsync(@"
			USE [pocFile];

			IF NOT EXISTS (select * from sysobjects where name='User' and xtype='U')
				CREATE TABLE [User] (
					Id INT PRIMARY KEY IDENTITY(1,1),
					Name VARCHAR(200),
					Email VARCHAR(200),
					ProductCode INT,
					PaymentDate DATE,
					PaymentValue DECIMAL(10, 3)
				)");

	private async Task InsertDatasAsync()
	{
        const string INSERT_SQL = @"
			USE [pocFile];

			INSERT INTO [dbo].[User]
				([Name]
				,[Email]
				,[ProductCode]
				,[PaymentDate]
				,[PaymentValue])
			VALUES
				(@Name
				,@Email
				,@ProductCode
				,@PaymentDate
				,@PaymentValue)";

		var users = new Faker<UserModel>()
			.CustomInstantiator(faker => new UserModel(
				name: faker.Name.FullName(),
				email: faker.Internet.Email(),
				productCode: faker.Random.Int(),
				paymentDate: faker.Date.Future(),
				paymentValue: faker.Random.Decimal()))
			.Generate(1_000_000);

		foreach (var user in users)
		{
            await _dbContext.Connection.ExecuteAsync(INSERT_SQL, new
            {
                user.Name,
                user.Email,
                user.ProductCode,
                user.PaymentDate,
                user.PaymentValue
            });
        }
	}

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
