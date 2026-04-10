using System.Collections.Generic;
using System.Linq;

namespace IronStrata.Scripts.Core.Types;

public static class OptionExtensions
{
    /// <summary>
    /// Converts a reference type to an Option, where null is None and not-null is Some.
    /// </summary>
    public static Option<T> ToOption<T>(this T value) where T : class
    {
        return value == null ? Option<T>.None : Option<T>.Some(value);
    }

    /// <summary>
    /// Converts a nullable value type to an Option.
    /// </summary>
    public static Option<T> ToOption<T>(this T? value) where T : struct
    {
        return value.HasValue ? Option<T>.Some(value.Value) : Option<T>.None;
    }

    /// <summary>
    /// Returns the first element of a sequence as an Option, or None if the sequence is empty.
    /// </summary>
    public static Option<T> FirstOptional<T>(this IEnumerable<T> source)
    {
        foreach (var item in source) return Option<T>.Some(item);
        return Option<T>.None;
    }

    /// <summary>
    /// Combines two Options into an Option of a tuple. Returns None if either Option is None.
    /// </summary>
    public static Option<(T1, T2)> Zip<T1, T2>(this Option<T1> left, Option<T2> right)
    {
        if (left.IsSome && right.IsSome) return Option<(T1, T2)>.Some((left.Unwrap(), right.Unwrap()));
        return Option<(T1, T2)>.None;
    }
}
