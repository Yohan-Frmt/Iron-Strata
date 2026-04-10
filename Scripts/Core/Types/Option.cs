using System;
using System.Collections.Generic;

namespace IronStrata.Scripts.Core.Types;

/// <summary>
/// Represents an optional value: every Option is either Some and contains a value, or None, and does not.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
public readonly struct Option<T> : IEquatable<Option<T>>
{
    private readonly bool _isSome;
    private readonly T _value;

    private Option(T value)
    {
        _isSome = true;
        _value = value;
    }

    /// <summary>
    /// Returns true if the option is a Some value.
    /// </summary>
    public bool IsSome => _isSome;

    /// <summary>
    /// Returns true if the option is a None value.
    /// </summary>
    public bool IsNone => !_isSome;

    /// <summary>
    /// Creates a Some value.
    /// </summary>
    public static Option<T> Some(T value) => new(value);

    /// <summary>
    /// Creates a None value.
    /// </summary>
    public static Option<T> None => default;

    /// <summary>
    /// Returns the contained Some value, or throws an InvalidOperationException if the value is None.
    /// </summary>
    public T Unwrap() => _isSome ? _value : throw new InvalidOperationException("Called Option.Unwrap() on a None value.");

    /// <summary>
    /// Returns the contained Some value or a provided default.
    /// </summary>
    public T UnwrapOr(T defaultValue) => _isSome ? _value : defaultValue;

    /// <summary>
    /// Returns the contained Some value or computes it from a closure.
    /// </summary>
    public T UnwrapOrElse(Func<T> orElse) => _isSome ? _value : orElse();

    /// <summary>
    /// Maps an Option<T> to Option<U> by applying a function to a contained value.
    /// </summary>
    public Option<U> Map<U>(Func<T, U> mapper) => _isSome ? Option<U>.Some(mapper(_value)) : Option<U>.None;

    /// <summary>
    /// Returns None if the option is None, otherwise calls binder with the wrapped value and returns the result.
    /// </summary>
    public Option<U> Bind<U>(Func<T, Option<U>> binder) => _isSome ? binder(_value) : Option<U>.None;

    /// <summary>
    /// Executes the appropriate action based on whether the option is Some or None.
    /// </summary>
    public void Match(Action<T> onSome, Action onNone)
    {
        if (_isSome) onSome(_value);
        else onNone();
    }

    /// <summary>
    /// Evaluates the appropriate function based on whether the option is Some or None.
    /// </summary>
    public U Match<U>(Func<T, U> onSome, Func<U> onNone) => _isSome ? onSome(_value) : onNone();

    public bool Equals(Option<T> other)
    {
        if (!_isSome && !other._isSome) return true;
        if (_isSome && other._isSome) return EqualityComparer<T>.Default.Equals(_value, other._value);
        return false;
    }

    public override bool Equals(object obj) => obj is Option<T> other && Equals(other);

    public override int GetHashCode() => _isSome ? _value.GetHashCode() : 0;

    public static bool operator ==(Option<T> left, Option<T> right) => left.Equals(right);

    public static bool operator !=(Option<T> left, Option<T> right) => !left.Equals(right);

    public static implicit operator Option<T>(T value) => value is null ? None : Some(value);
}

/// <summary>
/// Helper class for creating Option instances.
/// </summary>
public static class Option
{
    public static Option<T> Some<T>(T value) => Option<T>.Some(value);
    public static Option<T> None<T>() => Option<T>.None;
}
