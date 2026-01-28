# Rufus

While we wait for the implementation
of [C# discriminated unions by Microsoft](https://github.com/dotnet/csharplang/blob/main/meetings/working-groups/discriminated-unions/union-proposals-overview.md),
Inspired by [Rust's Result and Option types](https://doc.rust-lang.org/std/result/).
> This package requires C# 10 or later.

## Result

Defines a type to represent either a success or a failure that can be bound to two-path
execution methods as described in [Railway Oriented Programming](https://fsharpforfunandprofit.com/rop/).

> Although it does not make sense to return a null ``Result`` value, this type is declared
> as reference to allow an easier switch case pattern matching in order to extract underlying
> values.
>
> A common implementation option consist on declaring a ``Match`` method that receives
> expressions to handle both cases, but this implementation might fall into allocating lambda
> closures, so Rufus takes a more imperative approach like Rust.
>
> Microsoft implementation will allow discriminated unions to be declared as value types in the future
> which makes more sense.

### Initialization

Besides usual ```Result.Ok``` and ```Result.Error``` constructors, you can also initialize a result
by invoking helper functions declared in the static ``Result`` class:

```csharp
Result<int, string> ok = Result.Ok(42);
Result<int, string> error = Result.Error("Something went wrong");
```

This sugar syntax initialization for results makes an implicit cast from a value type to the
reference ``Result<T, TError>`` type.

### Pattern matching expressions

Generic result type implements either ``Result.Ok`` or ``Result.Error`` case to perform
switch less verbose by avoiding specifying both success and error types.

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
bool isSuccess = result.Switch(
    Result<int, string>.Ok => true,
    Result<int, string>.Error => false
);
assert.True(isSuccess);
```

Even you might take advantage of aliasing you result types.
