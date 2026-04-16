using System.Collections.Generic;

namespace IronStrata.Scripts.Core.Types;

public static class OptionExtensions
{
    /// <summary>
    /// Converts a reference type value to an Option. If the value is not null, wraps it in Some; otherwise, returns None.
    /// </summary>
    /// <typeparam name="T">The type of the reference class.</typeparam>
    /// <param name="value">The reference type value to convert to an Option.</param>
    /// <returns>An Option containing the value if it is not null, otherwise None.</returns>
    public static Option<T> ToOption<T>(this T value) where T : class
    {
        return value == null ? Option<T>.None : Option<T>.Some(value);
    }

    /// <summary>
    /// Converts a value of a reference type to an Option, wrapping it in Some if it is not null, otherwise returning None.
    /// </summary>
    /// <typeparam name="T">The type of the reference class.</typeparam>
    /// <param name="value">The reference value to convert to an Option.</param>
    /// <returns>An Option containing the value if it is not null, otherwise None.</returns>
    public static Option<T> ToOption<T>(this T? value) where T : struct
    {
        return value.HasValue ? Option<T>.Some(value.Value) : Option<T>.None;
    }

    /// <summary>
    /// Retrieves the first element from a sequence as an Option.
    /// If the sequence contains at least one element, an Option wrapping the first element is returned.
    /// If the sequence is empty, an Option in the None state is returned.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
    /// <param name="source">The sequence to retrieve the first element from.</param>
    /// <returns>An Option containing the first element if the sequence is not empty, otherwise None.</returns>
    public static Option<T> FirstOptional<T>(this IEnumerable<T> source)
    {
        foreach (var item in source) return Option<T>.Some(item);
        return Option<T>.None;
    }

    /// <summary>
    /// Combines two Option values into a single Option that contains a tuple of both values, if both Options are in the Some state.
    /// Returns None if either of the Options is in the None state.
    /// </summary>
    /// <typeparam name="T1">The type of the value contained in the first Option.</typeparam>
    /// <typeparam name="T2">The type of the value contained in the second Option.</typeparam>
    /// <param name="left">The first Option value to combine.</param>
    /// <param name="right">The second Option value to combine.</param>
    /// <returns>An Option containing a tuple of the two values if both Options are Some, otherwise None.</returns>
    public static Option<(T1, T2)> Zip<T1, T2>(this Option<T1> left, Option<T2> right)
    {
        if (left.IsSome && right.IsSome) return Option<(T1, T2)>.Some((left.Unwrap(), right.Unwrap()));
        return Option<(T1, T2)>.None;
    }
}
