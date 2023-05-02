using BenchmarkDotNet.Running;
using poc_read_dynamic_file.Extensions;
using poc_read_dynamic_file.Models;
using poc_read_dynamic_file.Options;
using poc_read_dynamic_file.Service;
using System.Reflection.PortableExecutable;

Action<string, IDictionary<string, int>?, ColumnFileOption<string>> processSeparatorLine = (line, headers, options) =>
{
    if (headers is null)
    {
        headers = line.MapFields(options, '|');
        return;
    }

    UserModel user = new(line.Split('|'), headers, options);
    users.Add(user);
};


BenchmarkRunner.Run<MapWithSeparatorFileService>();
BenchmarkRunner.Run<MapWithPositionsFileService>();

await new MapWithSeparatorFileService().SeparatorWithStreamReaderAsync();
await new MapWithSeparatorFileService().SeparatorWithPipeReaderAsync();

await new MapWithPositionsFileService().PositionsWithStreamReaderAsync();
await new MapWithPositionsFileService().PositionsWithPipeReaderAsync();