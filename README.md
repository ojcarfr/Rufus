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

# Design principles

## Pattern matching expressions

> Following interface casing only applies up to dotnet 10. From dotnet 11, union types are used instead.

Generic result type implements either ``Ok<T>`` or ``Error<T>`` case to perform less verbose switch by
avoiding specifying both success and error types.

```csharp
Result<int, string> result = Result.Ok(42);
bool isSuccess = result.Switch(
    Result.Ok<int> => true,
    Result.Error<string> => false
);
Assert.True(isSuccess);
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
match the generic result type.

```csharp
Result<int, string> result = Result.Ok(42);
bool isSuccess = result switch {
    Result<int, string>.Ok => true,
    Result<int, string>.Error => false,
};
assert.True(isSuccess);
```

Even you might take advantage of aliasing you result types to avoid generic specification
on every switch expression.

According to benchmarks, the cost of matching to ```Result``` interface is almost inexistent:

| Method                           |     Mean |     Error |    StdDev | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
|----------------------------------|---------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| 'Switch by generic Result type'  | 1.880 ns | 0.0619 ns | 0.1422 ns |  1.01 |    0.11 | 0.0029 |      24 B |        1.00 |
| 'Switch by interface case types' | 1.986 ns | 0.0648 ns | 0.1849 ns |  1.06 |    0.13 | 0.0029 |      24 B |        1.00 |

## Asynchronous support

```Result<T, TError>``` defines ```AndThen``` and ```OrElse``` overloads in order to bind the two-path executions
to asynchronous methods, so do it allows asynchronous promises to be bound to any other function piping the execution
without awaiting them.

> Any uncaught exception will be propagated to the first await sentence.

Due to both ```Task``` and ```ValueTask``` are de-fact dotnet promise standards with its own benefits,
overloads are defined to both of these types instead of creating a new AsyncResult primitive. In this way,
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