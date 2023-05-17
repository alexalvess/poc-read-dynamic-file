using poc_read_dynamic_file.Options;

namespace poc_read_dynamic_file.Models;

public class UserModel
{
    public UserModel()
    {

    }

    public UserModel(string name, string email, int productCode, DateTime paymentDate, decimal paymentValue)
    {
        Name = name;
        Email = email;
        ProductCode = productCode;
        PaymentDate = paymentDate;
        PaymentValue = paymentValue;

        ValuesSQL = string.Format("('{0}','{1}',{2},'{3}',{4})", name, email, productCode, paymentDate, paymentValue);
    }

    public UserModel(string[] fields, IDictionary<string, int> headers, ColumnFileOption<string> option)
    {
        Name = fields[headers[option.Name]].Trim();
        Email = fields[headers[option.Email]].Trim();
        ProductCode = Convert.ToInt32(fields[headers[option.ProductCode]]);
        PaymentDate = Convert.ToDateTime(fields[headers[option.PaymentDate]]);
        PaymentValue = Convert.ToDecimal(fields[headers[option.PaymentValue]]);
    }

    public UserModel(string line, ColumnFileOption<PositionFieldOption> option)
    {
        Name = line.Substring(option.Name.Start, option.Name.Length).Trim();
        Email = line.Substring(option.Email.Start, option.Email.Length).Trim();
        ProductCode = Convert.ToInt32(line.Substring(option.ProductCode.Start, option.ProductCode.Length));
        PaymentDate = Convert.ToDateTime(line.Substring(option.PaymentDate.Start, option.PaymentDate.Length));
        PaymentValue = Convert.ToDecimal(line.Substring(option.PaymentValue.Start, option.PaymentValue.Length));
    }

    public string Name { get; init; }

    public string Email { get; init; }

    public int ProductCode { get; init; }

    public DateTime PaymentDate { get; init; }

    public decimal PaymentValue { get; init; }

    public string ValuesSQL { get; }
}
