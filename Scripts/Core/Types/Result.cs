using System;
using System.Collections.Generic;

namespace IronStrata.Scripts.Core.Types;

/// <summary>
/// Represents the outcome of an operation, which can either be a success containing a value of type T,
/// or a failure containing an error of type E.
/// </summary>
/// <typeparam name="T">The type of the value contained in a successful result.</typeparam>
/// <typeparam name="E">The type of the error contained in a failed result.</typeparam>
public readonly struct Result<T, E> : IEquatable<Result<T, E>> {
    /// <summary>
    /// Holds the value associated with a successful result.
    /// </summary>
    /// <remarks>
    /// This field is only set when the result represents a successful operation (i.e., <c>IsOk</c> is <c>true</c>).
    /// When the result represents a failure, this field will be uninitialized or set to its default value.
    /// </remarks>
    private readonly T _value;

    /// <summary>
    /// Represents the error associated with a failed result.
    /// </summary>
    /// <remarks>
    /// This field is used internally to store the error value of type <typeparamref name="E"/>
    /// when the result represents a failure. It is only valid when <see cref="IsErr"/> is <c>true</c>.
    /// </remarks>
    private readonly E _error;

    /// <summary>
    /// Represents the result of an operation, which can either be a success value (Ok) or an error value (Err).
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="E">The type of the error value.</typeparam>
    private Result(T value) {
        IsOk = true;
        _value = value;
        _error = default;
    }

    /// <summary>
    /// Represents the result of an operation, which can either be a success (Ok) or an error (Err).
    /// </summary>
    /// <typeparam name="T">The type of the successful value.</typeparam>
    /// <typeparam name="E">The type of the error value.</typeparam>
    private Result(E error) {
        IsOk = false;
        _value = default;
        _error = error;
    }

    /// <summary>
    /// Indicates whether the operation result is successful.
    /// </summary>
    /// <remarks>
    /// This property returns <c>true</c> if the result represents a successful operation,
    /// and <c>false</c> if the result represents a failure. It is primarily used to
    /// check the status of the operation in the <see cref="Result{T, E}"/> type.
    /// </remarks>
    public bool IsOk { get; }

    /// <summary>
    /// Indicates whether the result represents a failed operation.
    /// </summary>
    /// <remarks>
    /// This property is used to determine if the result contains an error of type <typeparamref name="E"/>.
    /// It returns <c>true</c> when the result represents failure, and <c>false</c> when it represents success.
    /// </remarks>
    public bool IsErr => !IsOk;

    /// <summary>
    /// Creates an Ok result containing a success value.
    /// </summary>
    /// <param name="value">The success value to include in the result.</param>
    /// <returns>A Result representing a successful operation with the provided value.</returns>
    public static Result<T, E> Ok(T value) => new(value);

    /// <summary>
    /// Creates a failed result containing the specified error value.
    /// </summary>
    /// <param name="error">The error value to include in the result.</param>
    /// <returns>A Result representing a failed operation with the provided error.</returns>
    public static Result<T, E> Err(E error) => new(error);

    /// <summary>
    /// Extracts the value contained in a successful result.
    /// </summary>
    /// <returns>The value of the result if it represents a successful operation.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when attempting to unwrap a result that contains an error.
    /// </exception>
    public T Unwrap() =>
        IsOk ? _value : throw new InvalidOperationException($"Called Result.Unwrap() on an Err value: {_error}");

    /// <summary>
    /// Returns the contained error value if the result is an Err, or throws an InvalidOperationException if the result is Ok.
    /// </summary>
    /// <returns>The error value contained in the Err result.</returns>
    public E UnwrapErr() =>
        !IsOk ? _error : throw new InvalidOperationException($"Called Result.UnwrapErr() on an Ok value: {_value}");

    /// <summary>
    /// Returns the contained Ok value if the result is Ok; otherwise, returns the specified default value.
    /// </summary>
    /// <param name="defaultValue">The value to return if the result is Err.</param>
    /// <returns>The Ok value if the result is Ok; otherwise, the specified default value.</returns>
    public T UnwrapOr(T defaultValue) => IsOk ? _value : defaultValue;

    /// <summary>
    /// Transforms the success value of the result using the provided mapping function, while preserving the error if present.
    /// </summary>
    /// <typeparam name="U">The type of the value returned by the mapping function.</typeparam>
    /// <param name="mapper">The function to apply to the success value if the result is Ok.</param>
    /// <returns>A new Result containing the transformed value if the result was Ok, or the original error if the result was Err.</returns>
    public Result<U, E> Map<U>(Func<T, U> mapper) => IsOk ? Result<U, E>.Ok(mapper(_value)) : Result<U, E>.Err(_error);

    /// <summary>
    /// Transforms the error value of a failed result into a new error using the provided mapping function,
    /// while preserving the success result unchanged.
    /// </summary>
    /// <typeparam name="F">The type of the new error after transformation.</typeparam>
    /// <param name="mapper">A function that maps the current error value to a new error value.</param>
    /// <returns>A Result containing either the unchanged success value or the transformed error value.</returns>
    public Result<T, F> MapErr<F>(Func<E, F> mapper) =>
        IsOk ? Result<T, F>.Ok(_value) : Result<T, F>.Err(mapper(_error));

    /// <summary>
    /// Transforms the value contained in a successful result using the specified binder function,
    /// or propagates the error of a failed result without modification.
    /// </summary>
    /// <typeparam name="U">The type of the value in the result returned by the binder function.</typeparam>
    /// <param name="binder">A function that takes the successful value of the current result and returns a new result.</param>
    /// <returns>
    /// A new result containing the value returned by the binder function if the current result is successful,
    /// or the original error if the current result is a failure.
    /// </returns>
    public Result<U, E> Bind<U>(Func<T, Result<U, E>> binder) => IsOk ? binder(_value) : Result<U, E>.Err(_error);

    /// <summary>
    /// Executes the provided actions based on whether the result is a success or a failure.
    /// </summary>
    /// <param name="onOk">The action to execute if the result is a success.</param>
    /// <param name="onErr">The action to execute if the result is a failure.</param>
    public void Match(Action<T> onOk, Action<E> onErr) {
        if (IsOk) { onOk(_value); }
        else { onErr(_error); }
    }

    /// <summary>
    /// Transforms the outcome of a Result by applying one of two provided functions,
    /// depending on whether the Result is a success or a failure.
    /// </summary>
    /// <typeparam name="U">The type of the value returned by the provided functions.</typeparam>
    /// <param name="onOk">The function to apply if the Result is a success.</param>
    /// <param name="onErr">The function to apply if the Result is a failure.</param>
    /// <returns>The result of applying the appropriate function to the contained value
    /// or error, depending on whether the Result is a success or a failure.</returns>
    public U Match<U>(Func<T, U> onOk, Func<E, U> onErr) => IsOk ? onOk(_value) : onErr(_error);

    /// <summary>
    /// Converts the current Result into an Option containing the success value if the result is Ok,
    /// or a None if the result is Err.
    /// </summary>
    /// <returns>An Option that contains the success value if the result is Ok, or None otherwise.</returns>
    public Option<T> ToOption() => IsOk ? Option<T>.Some(_value) : Option<T>.None;

    /// <inheritdoc/>
    public bool Equals(Result<T, E> other) {
        if (IsOk != other.IsOk) { return false; }

        return IsOk
            ? EqualityComparer<T>.Default.Equals(_value, other._value)
            : EqualityComparer<E>.Default.Equals(_error, other._error);
    }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is Result<T, E> other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() =>
        IsOk
            ? HashCode.Combine(true, _value)
            : HashCode.Combine(false, _error);

    /// <summary>
    /// Determines whether two Result instances are equal by comparing their state and contained values or errors.
    /// </summary>
    /// <param name="left">The first Result instance to compare.</param>
    /// <param name="right">The second Result instance to compare.</param>
    /// <returns>True if the two instances represent the same state and value or error; otherwise, false.</returns>
    public static bool operator ==(Result<T, E> left, Result<T, E> right) => left.Equals(right);

    /// <summary>
    /// Determines whether two Result instances are not equal by comparing their state and contained values or errors.
    /// </summary>
    /// <param name="left">The first Result instance to compare.</param>
    /// <param name="right">The second Result instance to compare.</param>
    /// <returns>True if the two instances represent different states or values/errors; otherwise, false.</returns>
    public static bool operator !=(Result<T, E> left, Result<T, E> right) => !left.Equals(right);
}

/// <summary>
/// Provides static methods for creating success or error results using the generic <see cref="Result{T, E}"/> type.
/// </summary>
public static class Result {
    /// <summary>
    /// Creates a successful Result instance containing the specified value.
    /// </summary>
    /// <param name="value">The success value to be wrapped in the Result instance.</param>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="E">The type of the error value.</typeparam>
    /// <returns>A Result representing a successful operation with the provided value.</returns>
    public static Result<T, E> Ok<T, E>(T value) => Result<T, E>.Ok(value);

    /// <summary>
    /// Creates an Err result containing an error value.
    /// </summary>
    /// <param name="error">The error value to include in the result.</param>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="E">The type of the error value.</typeparam>
    /// <returns>A Result representing a failed operation with the provided error value.</returns>
    public static Result<T, E> Err<T, E>(E error) => Result<T, E>.Err(error);
}
