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

## Benchmark for Read Performance

Besides showing how we can read a file dynamically, here, we try to show how we can use 2 kinds of read files:
* [Stream Reader](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream?view=net-7.0): traditional way to read files
* [Pipeline Reader](https://learn.microsoft.com/en-us/dotnet/standard/io/pipelines): the way designed to make it easier to do high-performance I/O

_PS: For this example, we show the benchmark references about position strategy_

**About benchmark:**

Steps: Read the file, parse the data, and save it in the database.

<details>
<summary>10 Lines - 1 KB</summary>
    <table>
        <thead>
            <tr>
        	    <th>Strategy</th>
                <th>Mean</th>
                <th>Error</th>
                <th>Op/s</th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td>Stream</td>
                <td>275.1 ms</td>
                <td>NA</td>
                <td>3.635</td>
            </tr>
            <tr>
                <td>Pipe</td>
                <td>259.6 ms</td>
                <td>NA</td>
                <td>3.852</td>
            </tr>
        </tbody>
    </table>
</details>

<details>
<summary>100 Lines - 7 KB</summary>
    <table>
        <thead>
            <tr>
        	    <th>Strategy</th>
                <th>Mean</th>
                <th>Error</th>
                <th>Op/s</th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td>Stream</td>
                <td>375.8 ms</td>
                <td>NA</td>
                <td>2.661</td>
            </tr>
            <tr>
                <td>Pipe</td>
                <td>362.5 ms</td>
                <td>NA</td>
                <td>2.759</td>
            </tr>
        </tbody>
    </table>
</details>

<details>
<summary>1.000 Lines - 66 KB</summary>
    <table>
        <thead>
            <tr>
        	    <th>Strategy</th>
                <th>Mean</th>
                <th>Error</th>
                <th>Op/s</th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td>Stream</td>
                <td>1.575 s</td>
                <td>NA</td>
                <td>0.6350</td>
            </tr>
            <tr>
                <td>Pipe</td>
                <td>1.499 s</td>
                <td>NA</td>
                <td>0.6671</td>
            </tr>
        </tbody>
    </table>
</details>

<details>
<summary>10.000 Lines - 655 KB</summary>
    <table>
        <thead>
            <tr>
        	    <th>Strategy</th>
                <th>Mean</th>
                <th>Error</th>
                <th>Op/s</th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td>Stream</td>
                <td>13.01 s</td>
                <td>NA</td>
                <td>0.0769</td>
            </tr>
            <tr>
                <td>Pipe</td>
                <td>12.62 s</td>
                <td>NA</td>
                <td>0.0793</td>
            </tr>
        </tbody>
    </table>
</details>

<details>
<summary>100.000 Lines - 6.543 KB</summary>
    <table>
        <thead>
            <tr>
        	    <th>Strategy</th>
                <th>Mean</th>
                <th>Error</th>
                <th>Op/s</th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td>Stream</td>
                <td>125.2 s</td>
                <td>NA</td>
                <td>0.0080</td>
            </tr>
            <tr>
                <td>Pipe</td>
                <td>148.7 s</td>
                <td>NA</td>
                <td>0.0067</td>
            </tr>
        </tbody>
    </table>
</details>

<details>
<summary>1.000.000 Lines - 65.430 KB</summary>
    <table>
        <thead>
            <tr>
        	    <th>Strategy</th>
                <th>Mean</th>
                <th>Error</th>
                <th>Op/s</th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td>Stream</td>
                <td>22.48 m</td>
                <td>NA</td>
                <td>0.0007</td>
            </tr>
            <tr>
                <td>Pipe</td>
                <td>24.13 m</td>
                <td>NA</td>
                <td>0.0007</td>
            </tr>
        </tbody>
    </table>
</details>