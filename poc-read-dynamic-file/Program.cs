using BenchmarkDotNet.Running;
using poc_read_dynamic_file.Service.ReadUseCase;
using poc_read_dynamic_file.Service.WriteUseCase;
using poc_read_dynamic_file.Service.WriteUseCase.Database;

#if RELEASE

//BenchmarkRunner.Run<MapWithSeparatorFileService>();
//BenchmarkRunner.Run<MapWithPositionsFileService>();

BenchmarkRunner.Run<WriterFileService>();

#else

//using var seed = new Seed();
//await seed.DataSeedAsync();

using var service = new WriterFileService();
await service.WithPipelineAsync();
await service.WithStreamAsync();

//await new MapWithSeparatorFileService().SeparatorWithStreamReaderAsync();
//await new MapWithSeparatorFileService().SeparatorWithPipeReaderAsync();

//await new MapWithPositionsFileService().PositionsWithStreamReaderAsync();
//await new MapWithPositionsFileService().PositionsWithPipeReaderAsync();

#endif