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
    /// Returns the contained value if the option is in the Some state; otherwise, throws an InvalidOperationException.
    /// </summary>
    /// <returns>The contained value of type T.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the option is in the None state.</exception>
    public T Unwrap() => _isSome ? _value : throw new InvalidOperationException("Called Option.Unwrap() on a None value.");

    /// <summary>
    /// Represents an optional value: every Option is either Some and contains a value, or None and does not.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    public T UnwrapOr(T defaultValue) => _isSome ? _value : defaultValue;

    /// <summary>
    public T UnwrapOrElse(Func<T> orElse) => _isSome ? _value : orElse();

    /// <summary>
    /// Transforms the contained value in the Option if it is a Some, using the provided mapping function.
    /// Returns a new Option containing the result of the transformation, or None if the original Option is None.
    /// </summary>
    /// <typeparam name="U">The type of the value after the transformation.</typeparam>
    /// <param name="mapper">A function to transform the contained value.</param>
    /// <returns>
    /// A new Option containing the result of applying the mapping function to the contained value if Some,
    /// or None if the original Option is None.
    /// </returns>
    public Option<U> Map<U>(Func<T, U> mapper) => _isSome ? Option<U>.Some(mapper(_value)) : Option<U>.None;

    /// <summary>
    /// Transforms the current Option value by applying a provided binder function if the value is in the Some state,
    /// or returns None if the Option is in the None state.
    /// </summary>
    /// <typeparam name="U">The type of the Option result after applying the binder function.</typeparam>
    /// <param name="binder">A function to transform the value contained within the Some state to another Option of type U.</param>
    /// <returns>
    /// A new Option of type U resulting from the binder function if the Option is in the Some state,
    /// or None if the Option is in the None state.
    /// </returns>
    public Option<U> Bind<U>(Func<T, Option<U>> binder) => _isSome ? binder(_value) : Option<U>.None;

    /// <summary>
    /// Invokes the specified actions depending on whether the Option contains a value or not.
    /// </summary>
    /// <param name="onSome">The action to invoke if the Option contains a value.</param>
    /// <param name="onNone">The action to invoke if the Option does not contain a value. This parameter is optional.</param>
    public void Match(Action<T> onSome, Action onNone = null)
    {
        if (_isSome) onSome(_value);
        else onNone?.Invoke();
    }

    /// <summary>
    /// Executes one of the two provided functions based on the Option's state, returning the result of the executed function.
    /// </summary>
    /// <typeparam name="U">The return type of the functions.</typeparam>
    /// <param name="onSome">The function to be executed if the Option is in the "Some" state, receiving the stored value.</param>
    /// <param name="onNone">The function to be executed if the Option is in the "None" state.</param>
    /// <returns>The result of the executed function.</returns>
    public U Match<U>(Func<T, U> onSome, Func<U> onNone) => _isSome ? onSome(_value) : onNone();

    public bool Equals(Option<T> other) =>
        _isSome switch
        {
            false when !other._isSome => true,
            true when other._isSome => EqualityComparer<T>.Default.Equals(_value, other._value),
            _ => false
        };

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is Option<T> other && Equals(other);
    
    /// <inheritdoc/>
    public override int GetHashCode() => _isSome ? _value.GetHashCode() : 0;

    /// <summary>
    /// Determines whether two Option<T> instances are equal.
    /// </summary>
    /// <param name="left">The first Option<T> instance to compare.</param>
    /// <param name="right">The second Option<T> instance to compare.</param>
    /// <returns>true if both Option<T> instances are equal; otherwise, false.</returns>
    public static bool operator ==(Option<T> left, Option<T> right) => left.Equals(right);

    /// <summary>
    /// Determines whether two Option<T> instances are not equal.
    /// </summary>
    /// <param name="left">The first Option<T> instance to compare.</param>
    /// <param name="right">The second Option<T> instance to compare.</param>
    /// <returns>true if the Option<T> instances are not equal; otherwise, false.</returns>
    public static bool operator !=(Option<T> left, Option<T> right) => !left.Equals(right);

    /// <summary>
    /// Converts a value of type <typeparamref name="T"/> implicitly to an Option<T>.
    /// </summary>
    /// <param name="value">The value to convert. If the value is null, it will return None; otherwise, it will return Some with the value.</param>
    /// <returns>An Option<T> representing the provided value.</returns>
    public static implicit operator Option<T>(T value) => value is null ? None : Some(value);
}