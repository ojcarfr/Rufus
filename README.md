# Rufus

Define primitives to control operation results in a functional fashion *'a le Rust`*.

> Since dotnet does not infer types like Rust, and C# is still an OOP language, case types are still
> declared under a class like ```Result.Ok<T>```, this makes switch expressions longer than Rust.
> ```csharp
> _ = result switch
> {
>   Result.Ok<int>(var x) => x,
>   Result.Error<string> => 0,
> }
> ```
> If you want to have shorter switch cases, you can take advantage
>
of [static imports](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/using-directive#the-static-modifier).

## Result

Defines a type to represent either a success or a failure that can be bound to two-path
execution methods as described in [Railway Oriented Programming](https://fsharpforfunandprofit.com/rop/).

> Before dotnet 11, although it does not make sense to return a null ``Result`` value, this type is declared
> as reference to allow an easier switch case pattern matching in order to extract underlying
> values.
>
> A common implementation option consist on declaring a ``Match`` method that receives
> expressions to handle both cases, but this implementation might fall into allocating lambda
> closures, so Rufus takes a more imperative approach like Rust.
>
> From dotnet 11 onwards, union types are declared as value types, solving the issue.

### Initialization

Besides usual ```Result.Ok``` and ```Result.Error``` constructors, you can also initialize a result
by invoking helper functions declared in the static ``Result`` class:

```csharp
Result<int, string> ok = Result.Ok(42);
Result<int, string> error = Result.Error("Something went wrong");
```

> Previously to dotnet 11, ```Result.Success``` and ```Result.Failure``` should be used.

This sugar syntax initialization for results makes an implicit cast from a value type to the
reference ``Result<T, TError>`` type.

### Operations

- **Querying:** `IsOk`, `IsError`, `IsOkAnd`, `IsErrorAnd`
- **Inspecting:** `Inspect`, `InspectError`
- **Transforming:** `Map`, `MapOr`, `MapOrElse`, `MapError`, `Flatten`
- **Converting to `Option`:** `Ok` (returns `Option<T>` discarding the error), `Err` (returns `Option<TError>` discarding the success value)
- **Combining:** `And`, `AndThen`, `Or`, `OrElse`
- **Unwrapping:** `Unwrap`, `UnwrapError`, `UnwrapOr`, `UnwrapOrElse`, `UnwrapOrDefault`

```csharp
static Result<int, string> ParsePositive(string input) =>
    int.TryParse(input, out int n) && n > 0
        ? Result.Ok(n)
        : Result.Error($"'{input}' is not a positive integer");

// Chaining, inspecting, transforming and unwrapping
int value = ParsePositive("21")
    .Inspect(n => Console.WriteLine($"Parsed: {n}"))
    .AndThen(n => n <= 100 ? Result.Ok(n * 2) : Result.Error("Too large"))
    .Map(n => n + 1)
    .InspectError(err => Console.WriteLine($"Failed: {err}"))
    .UnwrapOr(0); // 43

// Querying & fallback
bool isEven = ParsePositive("10").IsOkAnd(n => n % 2 == 0); // true
int fallback = ParsePositive("-5").OrElse(_ => Result.Ok(1)).Unwrap(); // 1

// Converting to Option and flattening
Option<int> maybeValue = ParsePositive("42").Ok(); // Some(42)
Option<string> maybeError = ParsePositive("bad").Err(); // Some("'bad' is not a positive integer")
Result<int, string> flattened = Result.Ok<Result<int, string>>(Result.Ok(7)).Flatten(); // Ok(7)
```

## Option

Defines a type representing an optional value: every ```Option<T>``` is either ```Some<T>```,
which contains a value, or ```None```, which does not.

> Before dotnet 11, `Option<T>` is declared as an abstract record with `Some` and `None` variants;
> from dotnet 11 onwards, it is declared as a `readonly struct` union type.

### Initialization

On dotnet 10+, you can initialize an option using the static helpers on ``Option``:

```csharp
Option<int> some = Option.Some(42);
Option<_> unit = Option.Some();
Option<int> none = Option.None();
```

### Operations

- **Querying:** `IsSome`, `IsNone`, `IsSomeAnd`, `IsNoneOr` (and `true`/`false` operators)
- **Inspecting & Filtering:** `Inspect`, `Filter`
- **Transforming:** `Map`, `MapOr`, `MapOrElse`, `MapOrDefault`, `Flatten`, `Unzip`, `Transpose`
- **Converting to `Result`:** `OkOr`, `OkOrElse`
- **Combining:** `And`, `AndThen`, `Or`, `OrElse`, `Xor`, `Zip`, `ZipWith`, `Reduce`
- **Unwrapping:** `Expect`, `Unwrap`, `UnwrapOr`, `UnwrapOrElse`, `UnwrapOrDefault`
- **Enumerating:** `AsEnumerable`, `AsFlattenEnumerable`, `AsFlattenAsyncEnumerable`

```csharp
Option<string> name = Option.Some("  Rufus  ");

// Filtering, transforming, inspecting and unwrapping
int length = name
    .Map(s => s.Trim())
    .Filter(s => s.Length > 0)
    .Inspect(s => Console.WriteLine($"Name: {s}"))
    .MapOr(s => s.Length, 0); // 5

// Combining options
Option<(int, string)> zipped = Option.Some(1).Zip(Option.Some("hi")); // Some((1, "hi"))
(Option<int> left, Option<string> right) = zipped.Unzip();            // (Some(1), Some("hi"))
Option<int> reduced = Option.Some(12).Reduce(Option.Some(17), (a, b) => a + b); // Some(29)
Option<int> exclusive = Option.Some(2).Xor(Option.None());            // Some(2)

// Converting to Result and transposing
Result<int, string> asResult = Option.Some(5).OkOr("missing"); // Ok(5)
Result<Option<int>, string> transposed = Option.Some<Result<int, string>>(Result.Ok(5)).Transpose(); // Ok(Some(5))

// Flattening and enumerating
Option<int> flat = Option.Some<Option<int>>(Option.Some(6)).Flatten(); // Some(6)
IEnumerable<int> items = Option.Some<IEnumerable<int>>([1, 2, 3]).AsFlattenEnumerable(); // [1, 2, 3]
```

# Design principles

## Pattern matching expressions

> Following interface casing only applies up to dotnet 10. From dotnet 11, union types are used instead.

Generic result type implements either ``Ok<T>`` or ``Error<T>`` case to perform less verbose switch by
avoiding specifying both success and error types. Similarly, ``Option<T>`` matches against ``Option.Some<T>`` and ``Option.None``:

```csharp
Result<int, string> result = Result.Ok(42);
bool isSuccess = result switch
{
    Result.Ok<int>(var x) => x > 0,
    Result.Error<string> => false,
};
Assert.True(isSuccess);

Option<int> option = Option.Some(42);
int valueOrZero = option switch
{
    Option.Some<int>(var x) => x,
    Option.None => 0,
};
Assert.Equal(42, valueOrZero);
```

Relaying on the case interface type allows to match contravariant matches:

```csharp
Result<int, object> result = Result.Error("Expected error");
bool matched = result switch
{
    Result.Error<Exception> => false,
    Result.Error<string> => true,
    _ => false
};
Assert.True(matched);
```

Take in account that a cast to the interface is required. If you want to avoid it, just
match the generic result type (on dotnet 10 and earlier):

```csharp
Result<int, string> result = Result.Ok(42);
bool isSuccess = result switch
{
    Result<int, string>.Ok => true,
    Result<int, string>.Error => false,
};
Assert.True(isSuccess);
```

Even you might take advantage of aliasing your result types to avoid generic specification
on every switch expression, or using `using static System.Result;` / `using static System.Option;`
to shorten case names:

```csharp
using static System.Option;

Option<string> greeting = Option.Some("hello");
string text = greeting switch
{
    Some<string>(var s) => s,
    None => string.Empty,
};
```

According to benchmarks, the cost of matching to ```Result``` interface is almost inexistent:

| Method                           |     Mean |     Error |    StdDev | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
|----------------------------------|---------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| 'Switch by generic Result type'  | 1.880 ns | 0.0619 ns | 0.1422 ns |  1.01 |    0.11 | 0.0029 |      24 B |        1.00 |
| 'Switch by interface case types' | 1.986 ns | 0.0648 ns | 0.1849 ns |  1.06 |    0.13 | 0.0029 |      24 B |        1.00 |

## Asynchronous support

Both ```Result<T, TError>``` and ```Option<T>``` define ```AndThen``` and ```OrElse``` overloads in order to bind
two-path executions to asynchronous methods, allowing asynchronous promises to be bound to any other function
piping the execution without awaiting them.

```csharp
// Piping async Result<T, TError> without intermediate awaits
Result<string, string> finalResult = await FetchUserIdAsync()
    .AndThen(id => LoadProfileAsync(id))
    .OrElse(err => Task.FromResult(Result.Ok(Profile.Guest)));

// Piping async Option<T> without intermediate awaits
Option<string> finalOption = await FindCachedNameAsync()
    .AndThen(name => NormalizeAsync(name))
    .OrElse(() => ValueTask.FromResult(Option.Some("anonymous")));
```

> Any uncaught exception will be propagated to the first await sentence.

Due to both ```Task``` and ```ValueTask``` are de-fact dotnet promise standards with its own benefits,
overloads are defined to both of these types instead of creating new AsyncResult/AsyncOption primitives. In this way,
when needed, you can take advantage of ValueTask freely.

> When binding asynchronous promise, the implementation checks whether the promise were already finished,
> returning a new promise without creating the state machine.

## Uninitiated monads

Before dotnet 11, monads are declared as references, so even nullable is set, `null` reference can be
received. Furthermore, since case values are declared as interfaces (`closed` hierarchies are available
in dotnet 11), an unknown implementation might be received.

On the other hand, from dotnet 11 onwards, monads are declared as value types that can be received
uninitiated (`default`).

To keep consistent behavior, invoke operations on uninitiated monads will result in a
[SwitchExpressionException](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.switchexpressionexception?view=net-10.0),
except in fallback operations does not map any wrapped value (when a fallback default value is provided).