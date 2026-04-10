using System;
using System.Collections.Generic;

namespace IronStrata.Scripts.Core.Types;

/// <summary>
/// Result is a type that represents either success (Ok) or failure (Err).
/// </summary>
/// <typeparam name="T">The type of the success value.</typeparam>
/// <typeparam name="E">The type of the error value.</typeparam>
public readonly struct Result<T, E> : IEquatable<Result<T, E>>
{
    private readonly bool _isOk;
    private readonly T _value;
    private readonly E _error;

    private Result(T value)
    {
        _isOk = true;
        _value = value;
        _error = default;
    }

    private Result(E error)
    {
        _isOk = false;
        _value = default;
        _error = error;
    }

    /// <summary>
    /// Returns true if the result is Ok.
    /// </summary>
    public bool IsOk => _isOk;

    /// <summary>
    /// Returns true if the result is Err.
    /// </summary>
    public bool IsErr => !_isOk;

    /// <summary>
    /// Creates an Ok result.
    /// </summary>
    public static Result<T, E> Ok(T value) => new(value);

    /// <summary>
    /// Creates an Err result.
    /// </summary>
    public static Result<T, E> Err(E error) => new(error);

    /// <summary>
    /// Returns the contained Ok value, or throws an InvalidOperationException if the result is Err.
    /// </summary>
    public T Unwrap() => _isOk ? _value : throw new InvalidOperationException($"Called Result.Unwrap() on an Err value: {_error}");

    /// <summary>
    /// Returns the contained Err value, or throws an InvalidOperationException if the result is Ok.
    /// </summary>
    public E UnwrapErr() => !_isOk ? _error : throw new InvalidOperationException($"Called Result.UnwrapErr() on an Ok value: {_value}");

    /// <summary>
    /// Returns the contained Ok value or a provided default.
    /// </summary>
    public T UnwrapOr(T defaultValue) => _isOk ? _value : defaultValue;

    /// <summary>
    /// Maps a Result&lt;T, E&gt; to Result&lt;U, E&gt; by applying a function to a contained Ok value.
    /// </summary>
    public Result<U, E> Map<U>(Func<T, U> mapper) => _isOk ? Result<U, E>.Ok(mapper(_value)) : Result<U, E>.Err(_error);

    /// <summary>
    /// Maps a Result&lt;T, E&gt; to Result&lt;T, F&gt; by applying a function to a contained Err value.
    /// </summary>
    public Result<T, F> MapErr<F>(Func<E, F> mapper) => _isOk ? Result<T, F>.Ok(_value) : Result<T, F>.Err(mapper(_error));

    /// <summary>
    /// Returns Err if the result is Err, otherwise calls binder with the wrapped value and returns the result.
    /// </summary>
    public Result<U, E> Bind<U>(Func<T, Result<U, E>> binder) => _isOk ? binder(_value) : Result<U, E>.Err(_error);

    /// <summary>
    /// Executes the appropriate action based on whether the result is Ok or Err.
    /// </summary>
    public void Match(Action<T> onOk, Action<E> onErr)
    {
        if (_isOk) onOk(_value);
        else onErr(_error);
    }

    /// <summary>
    /// Evaluates the appropriate function based on whether the result is Ok or Err.
    /// </summary>
    public U Match<U>(Func<T, U> onOk, Func<E, U> onErr) => _isOk ? onOk(_value) : onErr(_error);

    /// <summary>
    /// Converts from Result&lt;T, E&gt; to Option&lt;T&gt;.
    /// </summary>
    public Option<T> ToOption() => _isOk ? Option<T>.Some(_value) : Option<T>.None;

    public bool Equals(Result<T, E> other)
    {
        if (_isOk != other._isOk) return false;
        return _isOk 
            ? EqualityComparer<T>.Default.Equals(_value, other._value) 
            : EqualityComparer<E>.Default.Equals(_error, other._error);
    }

    public override bool Equals(object obj) => obj is Result<T, E> other && Equals(other);

    public override int GetHashCode() => _isOk 
        ? HashCode.Combine(true, _value) 
        : HashCode.Combine(false, _error);

    public static bool operator ==(Result<T, E> left, Result<T, E> right) => left.Equals(right);

    public static bool operator !=(Result<T, E> left, Result<T, E> right) => !left.Equals(right);
}

/// <summary>
/// Helper class for creating Result instances.
/// </summary>
public static class Result
{
    public static Result<T, E> Ok<T, E>(T value) => Result<T, E>.Ok(value);
    public static Result<T, E> Err<T, E>(E error) => Result<T, E>.Err(error);
}
