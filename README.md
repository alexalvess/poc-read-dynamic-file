# Read dynamic Files (PoC)

This project has an objective to show how can we build a code to perform when we don't know the files behaviors, that we receive.

## About the Files and Map

First, we have a model class that we use to map file content into this class:

```csharp
public class UserModel
{
    public string Name { get; init; }

    public string Email { get; init; }

    public int ProductCode { get; init; }

    public DateTime PaymentDate { get; init; }

    public decimal PaymentValue { get; init; }
}
```