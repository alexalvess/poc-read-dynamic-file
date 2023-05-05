using Bogus;
using Bogus.DataSets;
using Dapper;
using poc_read_dynamic_file.Models;
using System.Globalization;

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

		var users = new Faker<UserModel>("pt_BR")
			.CustomInstantiator(faker => new UserModel(
				name: faker.Name.FirstName(),
				email: faker.Internet.Email(),
				productCode: faker.Random.Int(1, 9999),
				paymentDate: faker.Date.Future(),
				paymentValue: faker.Random.Decimal()))
			.Generate(500_000);

		foreach (var sql in GetSqlsInBatches(users))
			await _dbContext.Connection.ExecuteAsync(sql);
	}

    private IList<string> GetSqlsInBatches(IList<UserModel> users)
    {
        var insertSql = @"USE [pocFile];
			INSERT INTO [dbo].[User]
				([Name]
				,[Email]
				,[ProductCode]
				,[PaymentDate]
				,[PaymentValue]) VALUES ";
        var valuesSql =@"(
				'{0}'
				,'{1}'
				,{2}
				,'{3}'
				,{4})";
        var batchSize = 10_000;

        var sqlsToExecute = new List<string>();
        var numberOfBatches = (int)Math.Ceiling((double)users.Count / batchSize);

        CultureInfo ci = new CultureInfo("en-US");
        Thread.CurrentThread.CurrentCulture = ci;
        Thread.CurrentThread.CurrentUICulture = ci;

        for (int i = 0; i < numberOfBatches; i++)
        {
            var userToInsert = users.Skip(i * batchSize).Take(batchSize);
            var valuesToInsert = userToInsert.Select(u => string.Format(valuesSql, u.Name, u.Email, u.ProductCode, u.PaymentDate, u.PaymentValue));
            sqlsToExecute.Add(insertSql + string.Join(',', valuesToInsert));
        }

        return sqlsToExecute;
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
