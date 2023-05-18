using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Engines;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using poc_read_dynamic_file.Infra.Databases.Contexts;
using poc_read_dynamic_file.Infra.Databases.Repositories;
using poc_read_dynamic_file.Models;
using poc_read_dynamic_file.Options;
using poc_read_dynamic_file.Service.Extensions;
using poc_read_dynamic_file.Service.ReadUseCase;
using System.Reflection;

namespace poc_read_dynamic_file;

[SimpleJob(RunStrategy.ColdStart, iterationCount: 1)]
[AllStatisticsColumn]
public class StartupFactory
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public StartupFactory()
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var serviceCollections = new ServiceCollection();

        serviceCollections
            .AddOptions<ColumnFileOption<string>>()
            .Bind(_configuration.GetSection("ColumnSeparateFileOptions"));

        serviceCollections
            .AddOptions<ColumnFileOption<PositionFieldOption>>()
            .Bind(_configuration.GetSection("ColumnPositionsFileOptions"));

        serviceCollections
            .AddOptions<FilePathOption>()
            .Bind(_configuration.GetSection(nameof(FilePathOption)));

        serviceCollections
            .AddScoped<IDbContext, DbContext>()
            .AddScoped<UserRepository>()
            .AddScoped<ReadFileService>()
            .AddScoped<MapWithPositionsFileService>()
            .AddScoped<MapWithSeparatorFileService>()
            .AddScoped(_ => _configuration);

        serviceCollections
            .AddMassTransit(cfg =>
            {
                cfg.SetKebabCaseEndpointNameFormatter();
                cfg.AddConsumers(Assembly.GetExecutingAssembly());

                cfg.UsingRabbitMq((context, bus) =>
                {
                    bus.Host(_configuration.GetConnectionString("MessageBus"));
                    bus.ConfigureEndpoints(context);
                });
            });

        _serviceProvider = serviceCollections.BuildServiceProvider();
    }

    [Benchmark]
    public async Task ReadFileWithStreamAsync()
    {
        var service = _serviceProvider.GetRequiredService<MapWithPositionsFileService>();
        await service.PositionsWithStreamReaderAsync(true);
    }

    [Benchmark]
    public async Task ReadFileWithPipeAsync()
    {
        var service = _serviceProvider.GetRequiredService<MapWithPositionsFileService>();
        await service.PositionsWithPipeReaderAsync(true);
    }
}