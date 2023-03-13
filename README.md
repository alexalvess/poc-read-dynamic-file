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

And we have these files:

#### File with Position

![Position Files](https://raw.githubusercontent.com/alexalvess/poc-read-dynamic-file/main/.assets/img/position-file-sample.png)

For this kind of file, we need to know what position starts the field and the length.

#### File with Separator

![Separator Files](https://github.com/alexalvess/poc-read-dynamic-file/blob/main/.assets/img/separator-file-sample.png?raw=true)

For this kind of file, we need just map the header to know the fields.

---

## Let's Code!