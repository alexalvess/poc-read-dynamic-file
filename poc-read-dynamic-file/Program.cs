using BenchmarkDotNet.Running;
using poc_read_dynamic_file;

#if RELEASE

BenchmarkRunner.Run<StartupFactory>();

#else

var factory = new StartupFactory();
factory.ConfigureServices();

await factory.ReadPositionFileWithStreamAsync();

#endif