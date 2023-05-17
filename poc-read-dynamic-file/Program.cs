﻿using BenchmarkDotNet.Running;
using poc_read_dynamic_file.Infra.Databases.Repositories;
using poc_read_dynamic_file.Service.ReadUseCase;
using poc_read_dynamic_file.Service.WriteUseCase;

#if RELEASE

BenchmarkRunner.Run<MapWithSeparatorFileService>(;
//BenchmarkRunner.Run<MapWithPositionsFileService>();

//BenchmarkRunner.Run<WriterFileService>();
BenchmarkRunner.Run<RepositoryDatabase>();

#else

using var seed = new Seed();
await seed.DataSeedAsync();

//using var service = new WriterFileService();
//await service.WithPipelineWriteAsync();
//await service.WithPipelineAdvanceAsync();
//await service.WithStreamAsync();

//await new MapWithSeparatorFileService().SeparatorWithStreamReaderAsync();
//await new MapWithSeparatorFileService().SeparatorWithPipeReaderAsync();

//await new MapWithPositionsFileService().PositionsWithStreamReaderAsync();
//await new MapWithPositionsFileService().PositionsWithPipeReaderAsync();

#endif