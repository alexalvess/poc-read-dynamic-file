using poc_read_dynamic_file.Options;

namespace poc_read_dynamic_file.Service.Extensions;

public static class FileExtensions
{
    public static IDictionary<string, int> MapFields(this string line, ColumnFileOption<string> columnFileOptions, char seprator)
    {
        var properties = columnFileOptions.GetType().GetProperties();
        var lineFields = line.Split(seprator).ToList();
        var positions = new Dictionary<string, int>(properties.Length);

        foreach (var property in properties)
        {
            if (property.GetValue(columnFileOptions) is not string columnValue)
                throw new InvalidDataException($"Column name not found: {property.Name}");

            var index = lineFields.FindIndex(column => column.Trim().Equals(columnValue));
            positions.Add(columnValue, index);
        }

        return positions;
    }
}