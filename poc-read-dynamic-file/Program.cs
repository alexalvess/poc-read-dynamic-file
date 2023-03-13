using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using poc_read_dynamic_file.Options;
using poc_read_dynamic_file.Service;

IConfigurationRoot configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
    .AddJsonFile("appsettings.json", false)
    .Build();

var builder = new ServiceCollection()
    .AddScoped<MapFileService>()
    .AddOptions<ColumnFileOption<string>>()
        .Bind(configuration.GetSection("ColumnSeparateFileOptions"))
        .Services
    .AddOptions<ColumnFileOption<PositionFieldOption>>()
        .Bind(configuration.GetSection("ColumnPositionsFileOptions"))
        .Services
    .BuildServiceProvider();

var service = builder.GetRequiredService<MapFileService>();
await service.MapSeparatorFileAsync(default);
await service.MapPositionsFileAsync(default);
