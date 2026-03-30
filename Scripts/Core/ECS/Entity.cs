using System;

namespace IronStrata.Scripts.Core.ECS;

public class Entity(int id) : IEquatable<Entity>
{
    public readonly int Id = id;

    public bool Equals(Entity other) => Id == other?.Id;
    public override bool Equals(object obj) => obj is Entity e && Equals(e);
    public override int GetHashCode() => Id;
    public override string ToString() => $"Entity {Id}";

    public static readonly Entity Null = new(-1);
    public bool IsNull => Id == -1;
}