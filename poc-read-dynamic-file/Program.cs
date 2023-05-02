using BenchmarkDotNet.Running;
using poc_read_dynamic_file.Service;


#if RELEASE
//BenchmarkRunner.Run<MapWithSeparatorFileService>();
BenchmarkRunner.Run<MapWithPositionsFileService>();
#else
await new MapWithSeparatorFileService().SeparatorWithStreamReaderAsync();
await new MapWithSeparatorFileService().SeparatorWithPipeReaderAsync();

await new MapWithPositionsFileService().PositionsWithStreamReaderAsync();
await new MapWithPositionsFileService().PositionsWithPipeReaderAsync();
#endif
