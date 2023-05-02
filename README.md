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

## Benchmark

Besides showing how we can read a file dynamically, here, we try to show how we can use 2 kinds of read files:
* [Stream Reader](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream?view=net-7.0): traditional way to read files
* [Pipeline Reader](https://learn.microsoft.com/en-us/dotnet/standard/io/pipelines): the way designed to make it easier to do high-performance I/O

_PS: For this example, we show the benchmark references about separator strategy_

**About benchmark:**

| Strategy          | Performance | Error    | StdDev   | Gen0  | Gen1 | Gen2   | Allocated |
|-------------------|------------:|----------|----------|------:|-----:|-------:|----------:|
| Stream Reader     | 157.86 ms   | 3.044 ms | 6.080 ms | 15750 | 5250 | 3000.0 | 82.76 MB  |
| Pipeline Reader   | 17.56 ms    | 0.339 ms | 0.391 ms | 1593  | 1125 | 562.5  | 8.28 MB   |

PS: This benchmark happened in a file that contains 155,241K of lines