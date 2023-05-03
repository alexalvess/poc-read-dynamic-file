using BenchmarkDotNet.Running;
using poc_read_dynamic_file.Service.ReadUseCase;
using poc_read_dynamic_file.Service.WriteUseCase.Database;

#if RELEASE

BenchmarkRunner.Run<MapWithSeparatorFileService>();
BenchmarkRunner.Run<MapWithPositionsFileService>();

#else

using var seed = new Seed();
await seed.DataSeedAsync();

//await new MapWithSeparatorFileService().SeparatorWithStreamReaderAsync();
//await new MapWithSeparatorFileService().SeparatorWithPipeReaderAsync();

//await new MapWithPositionsFileService().PositionsWithStreamReaderAsync();
//await new MapWithPositionsFileService().PositionsWithPipeReaderAsync();

#endif