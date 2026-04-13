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
    /// Safely destroys an entity if it exists.
    /// </summary>
    /// <param name="entity">The entity to destroy.</param>
    /// <returns>A Result indicating success or failure with reason.</returns>
    public Result<Unit, string> TryDestroyEntity(Entity entity)
    {
        if (!_alive.Contains(entity.Id))
            return Result<Unit, string>.Err($"Entity {entity.Id} is not alive");
        
        DestroyEntity(entity);
        return Result<Unit, string>.Ok(default);
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
    /// Safely adds a component to an entity only if the entity is alive.
    /// </summary>
    /// <typeparam name="T">The type of the component.</typeparam>
    /// <param name="entity">The target entity.</param>
    /// <param name="component">The component instance to add.</param>
    /// <returns>A Result indicating success or failure with reason.</returns>
    public Result<T, string> TryAdd<T>(Entity entity, T component) where T : IComponent
    {
        if (!_alive.Contains(entity.Id))
            return Result<T, string>.Err($"Entity {entity.Id} is not alive");
        Add(entity, component);
        return Result<T, string>.Ok(component);
    }

    
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
    /// Determines whether a specified entity possesses a component of the given type.
    /// </summary>
    /// <typeparam name="T">The type of the component to check for.</typeparam>
    /// <param name="entity">The entity to check for the presence of the component.</param>
    /// <returns>True if the entity has a component of the specified type; otherwise, false.</returns>
    public bool Has<T>(Entity entity) where T : IComponent
    {
        return _stores.TryGetValue(typeof(T), out var store) && store.ContainsKey(entity.Id);
    }

    /// <summary>
    /// Removes the component of the specified type from the given entity.
    /// </summary>
    /// <typeparam name="T">The type of the component to remove.</typeparam>
    /// <param name="entity">The entity from which the component will be removed.</param>
    public void Remove<T>(Entity entity) where T : IComponent
    {
        if (_stores.TryGetValue(typeof(T), out var store)) store.Remove(entity.Id);
    }
    
    /// <summary>
    /// Removes a specific component from an entity and returns it if it existed.
    /// </summary>
    /// <typeparam name="T">The type of the component.</typeparam>
    /// <param name="entity">The target entity.</param>
    /// <returns>An Option containing the removed component if it existed.</returns>
    public Option<T> RemoveAndGet<T>(Entity entity) where T : IComponent
    {
        var component = GetOptional<T>(entity);
        Remove<T>(entity);
        return component;
    }

    /// <summary>
    /// Retrieves the first entity that contains the specified component type, if one exists.
    /// </summary>
    /// <returns>An option containing the first entity with the specified component type, or none if no such entity exists.</returns>
    public Option<Entity> QueryFirst<T>() where T : IComponent
    {
        return Query<T>().FirstOptional();
    }

    /// <summary>
    /// Queries all entities in the world containing the specified component type.
    /// </summary>
    /// <typeparam name="T">The type of the component to query for.</typeparam>
    /// <returns>An enumerable of entities that contain the specified component type.</returns>
    public IEnumerable<Entity> Query<T>() where T : IComponent
    {
        if (!_stores.TryGetValue(typeof(T), out var store)) yield break;
        foreach (var id in store.Keys.ToList().Where(id => _alive.Contains(id)))
            yield return new Entity(id);
    }


    /// <summary>
    /// Queries the first entity containing the specified component type.
    /// </summary>
    /// <typeparam name="T">The type of the component to query for.</typeparam>
    /// <returns>An optional entity containing the component, or none if no such entity exists.</returns>
    public Option<Entity> QueryFirst<T1, T2>()
        where T1 : IComponent
        where T2 : IComponent
    {
        return Query<T1, T2>().FirstOptional();
    }

    /// <summary>
    /// Queries the world for all entities containing the specified component type.
    /// </summary>
    /// <typeparam name="T">The type of component to query for.</typeparam>
    /// <returns>An enumerable collection of entities containing the specified component.</returns>
    public IEnumerable<Entity> Query<T1, T2>()
        where T1 : IComponent
        where T2 : IComponent
    {
        return Query<T1>().Where(Has<T2>);
    }

    /// <summary>
    /// Queries the first entity that contains a component of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the component to query for.</typeparam>
    /// <returns>An optional entity that contains the specified component type, or none if no match is found.</returns>
    public Option<Entity> QueryFirst<T1, T2, T3>()
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
    {
        return Query<T1, T2, T3>().FirstOptional();
    }

    /// <summary>
    /// Retrieves a collection of entities that match the specified component types.
    /// </summary>
    /// <typeparam name="T1">The first component type to match.</typeparam>
    /// <typeparam name="T2">The second component type to match.</typeparam>
    /// <returns>An enumerable of entities containing the specified components.</returns>
    public IEnumerable<Entity> Query<T1, T2, T3>()
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
    {
        return Query<T1, T2>().Where(Has<T3>);
    }

    /// <summary>
    /// Gets all components of a specific type with their associated entities.
    /// </summary>
    /// <typeparam name="T">The type of the component.</typeparam>
    /// <returns>An enumerable of tuples containing the entity and its component.</returns>
    public IEnumerable<T> QueryWith<T>() where T : IComponent
    {
        if (!_stores.TryGetValue(typeof(T), out var store)) yield break;
        foreach (var kvp in store.Where(kvp => _alive.Contains(kvp.Key)))
            yield return (T)kvp.Value;
    }

    /// <summary>
    /// Queries all entities that have the specified components and retrieves the entities paired with the associated components.
    /// </summary>
    /// <typeparam name="T1">The type of the first component to query.</typeparam>
    /// <typeparam name="T2">The type of the second component to query.</typeparam>
    /// <returns>An enumerable of tuples containing the entity and its associated components.</returns>
    public IEnumerable<(T1 c1, T2 c2)> QueryWith<T1, T2>()
        where T1 : IComponent
        where T2 : IComponent =>
        Query<T1, T2>().Select(entity => (Get<T1>(entity), Get<T2>(entity)));

    /// <summary>
    /// Queries entities that contain the specified component type and retrieves both the entity and its associated component.
    /// </summary>
    /// <typeparam name="T">The type of the component to query.</typeparam>
    /// <returns>An enumerable of tuples, where each tuple contains an entity and its associated component of type <typeparamref name="T"/>.</returns>
    public IEnumerable<(T1 c1, T2 c2, T3 c3)> QueryWith<T1, T2, T3>()
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent =>
        Query<T1, T2, T3>().Select(entity => (Get<T1>(entity), Get<T2>(entity), Get<T3>(entity)));


    /// <summary>
    /// Retrieves the store for a specific component type, creating it if it does not already exist.
    /// </summary>
    /// <returns>A dictionary containing the components of the specified type.</returns>
    private Dictionary<int, IComponent> GetOrCreateStore<T>() where T : IComponent
    {
        if (_stores.TryGetValue(typeof(T), out var store)) return store;
        store = [];
        _stores[typeof(T)] = store;
        return store;
    }
}

/// <summary>
/// Unit type for Result types that don't need to return a value.
/// </summary>
public readonly struct Unit
{
    public static Unit Default => default;
}