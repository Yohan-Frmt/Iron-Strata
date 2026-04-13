using System;
using System.Collections.Generic;
using System.Linq;
using IronStrata.Scripts.Core.Types;

namespace IronStrata.Scripts.Core.ECS;

/// <summary>
/// The World class is the core container for the ECS.
/// It manages entities and their associated components.
/// </summary>
public class World
{
    private int _nextId;

    /// <summary>
    /// Tracks all currently active entity IDs.
    /// </summary>
    private readonly HashSet<int> _alive = [];

    /// <summary>
    /// Stores IDs of destroyed entities that can be reused for new ones.
    /// </summary>
    private readonly Queue<int> _recycled = new();

    /// <summary>
    /// Maps component types to their respective storage dictionaries.
    /// </summary>
    private readonly Dictionary<Type, Dictionary<int, IComponent>> _stores = new();

    /// <summary>
    /// Event triggered when a new entity is created.
    /// </summary>
    public event Action<Entity> OnEntityCreated;

    /// <summary>
    /// Event triggered when an entity is destroyed.
    /// </summary>
    public event Action<Entity> OnEntityDestroyed;

    /// <summary>
    /// Creates a new entity, reusing a recycled ID if possible.
    /// </summary>
    /// <returns>The newly created Entity.</returns>
    public Entity CreateEntity()
    {
        var id = _recycled.Count > 0 ? _recycled.Dequeue() : _nextId++;
        _alive.Add(id);
        var entity = new Entity(id);
        OnEntityCreated?.Invoke(entity);
        return entity;
    }

    /// <summary>
    /// Destroys an entity and removes all its associated components.
    /// </summary>
    /// <param name="entity">The entity to destroy.</param>
    public void DestroyEntity(Entity entity)
    {
        if (!_alive.Contains(entity.Id)) return;
        foreach (var store in _stores.Values) store.Remove(entity.Id);
        _alive.Remove(entity.Id);
        _recycled.Enqueue(entity.Id);
        OnEntityDestroyed?.Invoke(entity);
    }

    /// <summary>
    /// Checks if an entity is still active in the world.
    /// </summary>
    public bool IsAlive(Entity entity) => _alive.Contains(entity.Id);

    /// <summary>
    /// Gets the number of currently active entities.
    /// </summary>
    public int EntityCount => _alive.Count;

    /// <summary>
    /// Adds or updates a component for an entity.
    /// </summary>
    /// <typeparam name="T">The type of the component.</typeparam>
    /// <param name="entity">The target entity.</param>
    /// <param name="component">The component instance to add.</param>
    public void Add<T>(Entity entity, T component) where T : IComponent => GetOrCreateStore<T>()[entity.Id] = component;

    /// <summary>
    /// Retrieves a component from an entity as an Option.
    /// </summary>
    /// <typeparam name="T">The type of the component.</typeparam>
    /// <returns>An Option containing the component if found, or None otherwise.</returns>
    public Option<T> GetOptional<T>(Entity entity) where T : IComponent =>
        _stores.TryGetValue(typeof(T), out var store) && store.TryGetValue(entity.Id, out var raw)
            ? Option<T>.Some((T)raw)
            : Option<T>.None;

    /// <summary>
    /// Retrieves a component from an entity.
    /// </summary>
    /// <typeparam name="T">The type of the component.</typeparam>
    /// <returns>The requested component instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the entity does not have the component.</exception>
    public T Get<T>(Entity entity) where T : IComponent =>
        GetOptional<T>(entity).UnwrapOrElse(() => 
            throw new InvalidOperationException($"Entity {entity.Id} does not have component {typeof(T).Name}"));

    /// <summary>
    /// Attempts to retrieve a component from an entity.
    /// </summary>
    /// <typeparam name="T">The type of the component.</typeparam>
    /// <param name="entity">The target entity.</param>
    /// <param name="component">The output component instance if found.</param>
    /// <returns>True if the component exists for the entity, false otherwise.</returns>
    public bool TryGet<T>(Entity entity, out T component) where T : IComponent
    {
        if (_stores.TryGetValue(typeof(T), out var store) && store.TryGetValue(entity.Id, out var raw))
        {
            component = (T)raw;
            return true;
        }
        component = default;
        return false;
    }

    /// <summary>
    /// Checks if an entity has a specific component.
    /// </summary>
    public bool Has<T>(Entity entity) where T : IComponent
    {
        return _stores.TryGetValue(typeof(T), out var store) && store.ContainsKey(entity.Id);
    }

    /// <summary>
    /// Removes a specific component from an entity.
    /// </summary>
    public void Remove<T>(Entity entity) where T : IComponent
    {
        if (_stores.TryGetValue(typeof(T), out var store)) store.Remove(entity.Id);
    }

    /// <summary>
    /// Returns all entities that have the specified component.
    /// </summary>
    public IEnumerable<Entity> Query<T>() where T : IComponent
    {
        if (!_stores.TryGetValue(typeof(T), out var store)) yield break;
        foreach (var id in store.Keys.ToList().Where(id => _alive.Contains(id)))
            yield return new Entity(id);
    }

    /// <summary>
    /// Returns all entities that have both specified components.
    /// </summary>
    public IEnumerable<Entity> Query<T1, T2>()
        where T1 : IComponent
        where T2 : IComponent
    {
        return Query<T1>().Where(Has<T2>);
    }

    /// <summary>
    /// Returns all entities that have all three specified components.
    /// </summary>
    public IEnumerable<Entity> Query<T1, T2, T3>()
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
    {
        return Query<T1, T2>().Where(Has<T3>);
    }

    /// <summary>
    /// Retrieves or creates a component storage for a given type.
    /// </summary>
    private Dictionary<int, IComponent> GetOrCreateStore<T>() where T : IComponent
    {
        if (_stores.TryGetValue(typeof(T), out var store)) return store;
        store = [];
        _stores[typeof(T)] = store;
        return store;
    }
}