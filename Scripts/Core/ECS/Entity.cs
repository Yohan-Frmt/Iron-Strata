using System;

namespace IronStrata.Scripts.Core.ECS;

/// <summary>
/// Represents a structured, lightweight identifier for entities within an Entity Component System (ECS).
/// </summary>
/// <remarks>
/// The <see cref="Entity"/> struct is immutable and uniquely identifies an entity within the ECS framework.
/// It serves as a fundamental building block in ECS by associating components with an identifier.
/// </remarks>
public readonly struct Entity(int id) : IEquatable<Entity> {
    /// <summary>
    /// Gets the unique identifier associated with the current <see cref="Entity"/> instance.
    /// </summary>
    /// <returns>
    /// An integer value representing the identifier of this <see cref="Entity"/>.
    /// This identifier distinguishes entities within the ECS framework.
    /// </returns>
    public readonly int Id = id;

    /// <summary>
    /// Represents a non-existent or uninitialized <see cref="Entity"/> instance.
    /// </summary>
    /// <returns>
    /// An <see cref="Entity"/> with an <see cref="Id"/> of -1, used to signify a null or invalid entity.
    /// </returns>
    public static readonly Entity Null = new(-1);

    /// <summary>
    /// Gets a value indicating whether the entity is null.
    /// </summary>
    /// <returns>
    /// True if the entity is null (having an <see cref="Id"/> of -1); otherwise, false.
    /// </returns>
    public bool IsNull => Id == -1;

    /// <summary>
    /// Determines whether the current <see cref="Entity"/> instance is equal to another <see cref="Entity"/> instance.
    /// </summary>
    /// <param name="other">The other <see cref="Entity"/> instance to compare with this instance.</param>
    /// <returns>True if the current instance is equal to the other instance; otherwise, false.</returns>
    public bool Equals(Entity other) => Id == other.Id;

    /// <inheritdoc />
    public override bool Equals(object obj) => obj is Entity other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => Id;

    /// <inheritdoc />
    public override string ToString() => $"Entity {Id}";

    /// <summary>
    /// Compares two <see cref="Entity"/> instances for equality.
    /// </summary>
    /// <param name="left">The first entity to compare.</param>
    /// <param name="right">The second entity to compare.</param>
    /// <returns>True if both entities are equal; otherwise, false.</returns>
    public static bool operator ==(Entity left, Entity right) => left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="Entity"/> instances are not equal.
    /// </summary>
    /// <param name="left">The first entity to compare.</param>
    /// <param name="right">The second entity to compare.</param>
    /// <returns>True if the entities are not equal; otherwise, false.</returns>
    public static bool operator !=(Entity left, Entity right) => !left.Equals(right);
}
