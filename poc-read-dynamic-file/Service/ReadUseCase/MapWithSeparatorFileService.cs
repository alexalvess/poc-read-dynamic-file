using Microsoft.Extensions.Options;
using poc_read_dynamic_file.Models;
using poc_read_dynamic_file.Options;
using poc_read_dynamic_file.Service.Extensions;

namespace poc_read_dynamic_file.Service.ReadUseCase;

public class MapWithSeparatorFileService
{
    private readonly ColumnFileOption<string> _columnFileOption;
    private readonly ReadFileService _service;
    private readonly FilePathOption _filePathOption;

    private readonly Action<string, IDictionary<string, int>?, ColumnFileOption<string>, List<UserModel>> _processLine;

    public MapWithSeparatorFileService(
        IOptions<ColumnFileOption<string>> columnFileOption, 
        IOptions<FilePathOption> filePathOption, 
        ReadFileService service)
    {
        _service = service;

        _columnFileOption = columnFileOption.Value;
        _filePathOption = filePathOption.Value;

        _processLine = (line, headers, options, users) =>
        {
            if (headers is null)
            {
                headers = line.MapFields(options, '|');
                return;
            }

            UserModel user = new(line.Split('|'), headers, options);
            users.Add(user);
        };
    }

    public async Task SeparatorWithStreamReaderAsync()
    {
        using FileStream stream = new(_filePathOption.SeparatorFilePath, FileMode.Open, FileAccess.Read);
        List<UserModel> users = new();
        IDictionary<string, int>? headers = null;

        //await _service.StreamReaderAsync(
        //    stream: stream,
        //    proccessLine: line => _processLine(line, headers, _columnFileOption, users),
        //    cancellationToken: default);
    }

    public async Task SeparatorWithPipeReaderAsync()
    {
        using FileStream stream = new(_filePathOption.SeparatorFilePath, FileMode.Open, FileAccess.Read);
        List<UserModel> users = new();
        IDictionary<string, int>? headers = null;

        //await _service.PipeReaderAsync(
        //    stream: stream,
        //    proccessLine: line => _processLine(line, headers, _columnFileOption, users),
        //    cancellationToken: default);
    }
}