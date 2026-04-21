using System;
using System.Collections.Generic;

namespace IronStrata.Scripts.Core.Types;

/// <summary>
/// Represents an optional value: every Option is either Some and contains a value, or None, and does not.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
public readonly struct Option<T> : IEquatable<Option<T>> {
    /// <summary>
    /// Stores the value contained within the Option instance when it is in the Some state.
    /// </summary>
    private readonly T _value;

    /// <summary>
    /// Represents an optional value: every Option is either Some and contains a value, or None, and does not.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    private Option(T value) {
        IsSome = true;
        _value = value;
    }

    /// <summary>
    /// Returns true if the option is a Some value.
    /// </summary>
    public bool IsSome { get; }

    /// <summary>
    /// Returns true if the option is a None value.
    /// </summary>
    public bool IsNone => !IsSome;

    /// <summary>
    /// Creates an Option instance that represents a value that exists.
    /// </summary>
    /// <param name="value">The value to be wrapped in an Option.</param>
    /// <returns>An Option containing the provided value.</returns>
    public static Option<T> Some(T value) => new(value);

    /// <summary>
    /// Represents the absence of a value in the context of an Option type.
    /// </summary>
    public static Option<T> None => default;

    /// <summary>
    /// Returns the contained value if the option is in the Some state; otherwise, throws an InvalidOperationException.
    /// </summary>
    /// <returns>The contained value of type T.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the option is in the None state.</exception>
    public T Unwrap() =>
        IsSome ? _value : throw new InvalidOperationException("Called Option.Unwrap() on a None value.");

    /// <summary>
    /// Represents an optional value: every Option is either Some and contains a value, or None and does not.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    public T UnwrapOr(T defaultValue) => IsSome ? _value : defaultValue;

    /// <summary>
    /// Returns the contained value if the Option is Some, otherwise invokes the specified delegate and returns its result.
    /// </summary>
    /// <param name="orElse">A delegate that produces a value to return if the Option is None.</param>
    /// <returns>The contained value if the Option is Some; otherwise, the result of invoking the specified delegate.</returns>
    public T UnwrapOrElse(Func<T> orElse) => IsSome ? _value : orElse();

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
    public Option<U> Map<U>(Func<T, U> mapper) => IsSome ? Option<U>.Some(mapper(_value)) : Option<U>.None;

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
    public Option<U> Bind<U>(Func<T, Option<U>> binder) => IsSome ? binder(_value) : Option<U>.None;

    /// <summary>
    /// Invokes the specified actions depending on whether the Option contains a value or not.
    /// </summary>
    /// <param name="onSome">The action to invoke if the Option contains a value.</param>
    /// <param name="onNone">The action to invoke if the Option does not contain a value. This parameter is optional.</param>
    public void Match(Action<T> onSome, Action onNone = null) {
        if (IsSome) {
            onSome(_value);
        }
        else {
            onNone?.Invoke();
        }
    }

    /// <summary>
    /// Executes one of the two provided functions based on the Option's state, returning the result of the executed function.
    /// </summary>
    /// <typeparam name="U">The return type of the functions.</typeparam>
    /// <param name="onSome">The function to be executed if the Option is in the "Some" state, receiving the stored value.</param>
    /// <param name="onNone">The function to be executed if the Option is in the "None" state.</param>
    /// <returns>The result of the executed function.</returns>
    public U Match<U>(Func<T, U> onSome, Func<U> onNone) => IsSome ? onSome(_value) : onNone();

    /// <inheritdoc/>
    public bool Equals(Option<T> other) =>
        IsSome switch {
            false when !other.IsSome => true,
            true when other.IsSome => EqualityComparer<T>.Default.Equals(_value, other._value),
            _ => false
        };

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is Option<T> other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => IsSome ? _value.GetHashCode() : 0;

    /// <summary>
    /// Determines whether two Option&lt;T&gt; instances are equal.
    /// </summary>
    /// <param name="left">The first Option&lt;T&gt; instance to compare.</param>
    /// <param name="right">The second Option&lt;T&gt; instance to compare.</param>
    /// <returns>true if both Option&lt;T&gt; instances are equal; otherwise, false.</returns>
    public static bool operator ==(Option<T> left, Option<T> right) => left.Equals(right);

    /// <summary>
    /// Determines whether two Option&lt;T&gt; instances are not equal.
    /// </summary>
    /// <param name="left">The first Option&lt;T&gt; instance to compare.</param>
    /// <param name="right">The second Option&lt;T&gt; instance to compare.</param>
    /// <returns>true if the Option&lt;T&gt; instances are not equal; otherwise, false.</returns>
    public static bool operator !=(Option<T> left, Option<T> right) => !left.Equals(right);

    /// <summary>
    /// Converts a value of type <typeparamref name="T"/> implicitly to an Option&lt;T&gt;
    /// </summary>
    /// <param name="value">The value to convert. If the value is null, it will return None; otherwise, it will return Some with the value.</param>
    /// <returns>An Option&lt;T&gt; representing the provided value.</returns>
    public static implicit operator Option<T>(T value) => value is null ? None : Some(value);
}
