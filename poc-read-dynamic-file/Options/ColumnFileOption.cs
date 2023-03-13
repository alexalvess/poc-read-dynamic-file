using System.ComponentModel.DataAnnotations;

namespace poc_read_dynamic_file.Options;

public record ColumnFileOption<TColumnType>
{
    [Required]
    public TColumnType Name { get; init; }

    [Required]
    public TColumnType Email { get; init; }

    [Required]
    public TColumnType ProductCode { get; init; }

    [Required]
    public TColumnType PaymentDate { get; init; }
    
    [Required]
    public TColumnType PaymentValue { get; init; }
}