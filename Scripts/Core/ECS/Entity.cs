using System;

namespace IronStrata.Scripts.Core.ECS;

/// <summary>
/// A lightweight representation of an entity in the ECS.
/// An entity is essentially an ID that groups several components together.
/// </summary>
public readonly struct Entity(int id) : IEquatable<Entity>
{
    /// <summary>
    /// Unique identifier for this entity.
    /// </summary>
    public readonly int Id = id;

    /// <summary>
    /// A standardized Null entity representation.
    /// </summary>
    public static readonly Entity Null = new(-1);

    /// <summary>
    /// Returns true if this entity is the Null entity.
    /// </summary>
    public bool IsNull => Id == -1;

    /// <summary>
    /// Check if this entity is equal to another entity by comparing their IDs.
    /// </summary>
    public bool Equals(Entity other) => Id == other.Id;

    /// <inheritdoc />
    public override bool Equals(object obj) => obj is Entity other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => Id;

    /// <inheritdoc />
    public override string ToString() => $"Entity {Id}";

    public static bool operator ==(Entity left, Entity right) => left.Equals(right);
    public static bool operator !=(Entity left, Entity right) => !left.Equals(right);
}