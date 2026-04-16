using System;
using System.Collections.Generic;
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
    private readonly Dictionary<Type, IComponentStore> _stores = new();

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
    public void Add<T>(Entity entity, T component) where T : struct =>
        GetStore<T>().Add(entity.Id, component);

    /// <summary>
    /// Safely adds a component of type <typeparamref name="T"/> to the specified entity,
    /// returning the entity if successful or an error message if the entity is invalid.
    /// </summary>
    /// <typeparam name="T">The type of the component to add. Must be a value type.</typeparam>
    /// <param name="entity">The entity to which the component will be added.</param>
    /// <param name="component">The component to add.</param>
    /// <returns>
    /// A <see cref="Result{Entity, String}"/> instance containing the entity if the operation succeeds,
    /// or an error message if the entity is null or invalid.
    /// </returns>
    public Result<Entity, string> SafeAdd<T>(Entity entity, T component) where T : struct
    {
        if (entity.IsNull) return Result<Entity, string>.Err("L'entité est nulle");
        Add(entity, component);
        return Result<Entity, string>.Ok(entity);
    }

    /// <summary>
    /// Retrieves a component from an entity as an Option.
    /// </summary>
    /// <typeparam name="T">The type of the component.</typeparam>
    /// <returns>An Option containing the component if found, or None otherwise.</returns>
    public Option<T> GetOptional<T>(Entity entity) where T : struct
    {
        var store = GetStore<T>();
        return store.Has(entity.Id) ? Option<T>.Some(store.Get(entity.Id)) : Option<T>.None;
    }

    /// <summary>
    /// Retrieves a reference to a specific component of type <typeparamref name="T"/> associated with the given entity.
    /// </summary>
    /// <typeparam name="T">The type of the component to be retrieved.</typeparam>
    /// <param name="entity">The entity from which the component is to be retrieved.</param>
    /// <returns>A reference to the requested component.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the entity does not have the specified component.</exception>
    public ref T Get<T>(Entity entity) where T : struct => ref GetStore<T>().Get(entity.Id);

    /// <summary>
    /// Attempts to retrieve a component of the specified type <typeparamref name="T"/> associated with the given entity.
    /// </summary>
    /// <param name="entity">The entity for which to retrieve the component.</param>
    /// <typeparam name="T">The type of component to retrieve.</typeparam>
    /// <returns>An <see cref="Option{T}"/> containing the component if it exists, or <see cref="Option{T}.None"/> if it does not.</returns>
    public Option<T> TryGet<T>(Entity entity) where T : struct
    {
        var store = GetStore<T>();
        return store.Has(entity.Id) ? Option<T>.Some(store.Get(entity.Id)) : Option<T>.None;
    }

    /// <summary>
    /// Checks whether the specified entity has a component of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the component to check for.</typeparam>
    /// <param name="entity">The entity to check.</param>
    /// <returns>True if the entity contains a component of the specified type; otherwise, false.</returns>
    public bool Has<T>(Entity entity) where T : struct
    {
        return GetStore<T>().Has(entity.Id);
    }

    /// <summary>
    /// Removes the component of the specified type from the given entity.
    /// </summary>
    /// <typeparam name="T">The type of the component to remove.</typeparam>
    /// <param name="entity">The entity from which the component will be removed.</param>
    public void Remove<T>(Entity entity) where T : struct
    {
        if (_stores.TryGetValue(typeof(T), out var store)) store.Remove(entity.Id);
    }

    /// <summary>
    /// Removes a specific component from an entity and returns it if it existed.
    /// </summary>
    /// <typeparam name="T">The type of the component.</typeparam>
    /// <param name="entity">The target entity.</param>
    /// <returns>An Option containing the removed component if it existed.</returns>
    public Option<T> RemoveAndGet<T>(Entity entity) where T : struct
    {
        var component = GetOptional<T>(entity);
        Remove<T>(entity);
        return component;
    }


    /// <summary>
    /// Represents an action that operates on a single component within the ECS.
    /// </summary>
    /// <typeparam name="T1">The type of the component.</typeparam>
    public delegate void QueryAction<T1>(ref T1 c1);

    /// <summary>
    /// Represents an action that operates on pairs of components of specified types within the ECS.
    /// Used in queries to process entities that have both components.
    /// </summary>
    /// <typeparam name="T1">The type of the first component in the pair.</typeparam>
    /// <typeparam name="T2">The type of the second component in the pair.</typeparam>
    public delegate void QueryAction<T1, T2>(ref T1 c1, ref T2 c2);

    /// <summary>
    /// Represents an action that operates on three components of specified types within the ECS.
    /// </summary>
    /// <typeparam name="T1">The type of the first component.</typeparam>
    /// <typeparam name="T2">The type of the second component.</typeparam>
    /// <typeparam name="T3">The type of the third component.</typeparam>
    public delegate void QueryAction<T1, T2, T3>(ref T1 c1, ref T2 c2, ref T3 c3);

    /// <summary>
    /// Represents an action that operates on an entity and a single component within the ECS.
    /// </summary>
    public delegate void EntityQueryAction<T1>(Entity entity, ref T1 c1);

    /// <summary>
    /// Represents an action that operates on an entity and two components within the ECS.
    /// </summary>
    public delegate void EntityQueryAction<T1, T2>(Entity entity, ref T1 c1, ref T2 c2);

    /// <summary>
    /// Represents an action that operates on an entity and three components within the ECS.
    /// </summary>
    public delegate void EntityQueryAction<T1, T2, T3>(Entity entity, ref T1 c1, ref T2 c2, ref T3 c3);

    /// <summary>
    /// Executes an action for all entities that contain component type T1.
    /// </summary>
    public void ForEach<T1>(QueryAction<T1> action) where T1 : struct
    {
        var s1 = GetStore<T1>();
        for (var i = 0; i < s1.Count; i++)
            action(ref s1.GetByIndex(i));
    }

    /// <summary>
    /// Executes an action for all entities that contain component type T1, providing the entity reference.
    /// </summary>
    public void ForEach<T1>(EntityQueryAction<T1> action) where T1 : struct
    {
        var s1 = GetStore<T1>();
        for (var i = 0; i < s1.Count; i++)
            action(new Entity(s1.GetEntityIdAt(i)), ref s1.GetByIndex(i));
    }

    /// <summary>
    /// Executes an action for all entities that contain both component types T1 and T2.
    /// </summary>
    /// <param name="action">The action to execute for each matching entity. Provides references to the components of type T1 and T2.</param>
    /// <typeparam name="T1">The type of the first component.</typeparam>
    /// <typeparam name="T2">The type of the second component.</typeparam>
    public void ForEach<T1, T2>(QueryAction<T1, T2> action)
        where T1 : struct
        where T2 : struct
    {
        var s1 = GetStore<T1>();
        var s2 = GetStore<T2>();

        if (s1.Count <= s2.Count) {
            for (var i = 0; i < s1.Count; i++) {
                var id = s1.GetEntityIdAt(i);
                if (s2.Has(id)) action(ref s1.GetByIndex(i), ref s2.Get(id));
            }
        } else {
            for (var i = 0; i < s2.Count; i++) {
                var id = s2.GetEntityIdAt(i);
                if (s1.Has(id)) action(ref s1.Get(id), ref s2.GetByIndex(i));
            }
        }
    }

    /// <summary>
    /// Executes an action for all entities that contain both component types T1 and T2, providing the entity reference.
    /// </summary>
    public void ForEach<T1, T2>(EntityQueryAction<T1, T2> action)
        where T1 : struct
        where T2 : struct
    {
        var s1 = GetStore<T1>();
        var s2 = GetStore<T2>();

        if (s1.Count <= s2.Count) {
            for (var i = 0; i < s1.Count; i++) {
                var id = s1.GetEntityIdAt(i);
                if (s2.Has(id)) action(new Entity(id), ref s1.GetByIndex(i), ref s2.Get(id));
            }
        } else {
            for (var i = 0; i < s2.Count; i++) {
                var id = s2.GetEntityIdAt(i);
                if (s1.Has(id)) action(new Entity(id), ref s1.Get(id), ref s2.GetByIndex(i));
            }
        }
    }

    /// <summary>
    /// Executes an action for all entities that contain all three component types.
    /// </summary>
    public void ForEach<T1, T2, T3>(QueryAction<T1, T2, T3> action)
        where T1 : struct where T2 : struct where T3 : struct
    {
        var s1 = GetStore<T1>();
        var s2 = GetStore<T2>();
        var s3 = GetStore<T3>();
        var min = Math.Min(s1.Count, Math.Min(s2.Count, s3.Count));

        if (min == s1.Count) {
            for (var i = 0; i < s1.Count; i++) {
                var id = s1.GetEntityIdAt(i);
                if (s2.Has(id) && s3.Has(id)) action(ref s1.GetByIndex(i), ref s2.Get(id), ref s3.Get(id));
            }
        } else if (min == s2.Count) {
            for (var i = 0; i < s2.Count; i++) {
                var id = s2.GetEntityIdAt(i);
                if (s1.Has(id) && s3.Has(id)) action(ref s1.Get(id), ref s2.GetByIndex(i), ref s3.Get(id));
            }
        } else {
            for (var i = 0; i < s3.Count; i++) {
                var id = s3.GetEntityIdAt(i);
                if (s1.Has(id) && s2.Has(id)) action(ref s1.Get(id), ref s2.Get(id), ref s3.GetByIndex(i));
            }
        }
    }

    /// <summary>
    /// Executes an action for all entities that contain all three component types, providing the entity reference.
    /// </summary>
    public void ForEach<T1, T2, T3>(EntityQueryAction<T1, T2, T3> action)
        where T1 : struct where T2 : struct where T3 : struct
    {
        var s1 = GetStore<T1>();
        var s2 = GetStore<T2>();
        var s3 = GetStore<T3>();
        var min = Math.Min(s1.Count, Math.Min(s2.Count, s3.Count));

        if (min == s1.Count) {
            for (var i = 0; i < s1.Count; i++) {
                var id = s1.GetEntityIdAt(i);
                if (s2.Has(id) && s3.Has(id)) action(new Entity(id), ref s1.GetByIndex(i), ref s2.Get(id), ref s3.Get(id));
            }
        } else if (min == s2.Count) {
            for (var i = 0; i < s2.Count; i++) {
                var id = s2.GetEntityIdAt(i);
                if (s1.Has(id) && s3.Has(id)) action(new Entity(id), ref s1.Get(id), ref s2.GetByIndex(i), ref s3.Get(id));
            }
        } else {
            for (var i = 0; i < s3.Count; i++) {
                var id = s3.GetEntityIdAt(i);
                if (s1.Has(id) && s2.Has(id)) action(new Entity(id), ref s1.Get(id), ref s2.Get(id), ref s3.GetByIndex(i));
            }
        }
    }

    /// <summary>
    /// Queries the world for the first entity containing the specified component type.
    /// </summary>
    /// <typeparam name="T">The type of the component to query for.</typeparam>
    /// <returns>An option containing the first entity with the specified component type, or none if no such entity exists.</returns>
    public Option<Entity> QueryFirst<T>() where T : struct
    {
        var s = GetStore<T>();
        foreach (var id in _alive)
            if (s.Has(id))
                return Option<Entity>.Some(new Entity(id));
        return Option<Entity>.None;
    }

    
    /// <summary>
    ///  Queries the world for the first entity containing the specified component types.
    /// </summary>
    /// <typeparam name="T1">The first component type to query for.</typeparam>
    /// <typeparam name="T2">The second component type to query for.</typeparam>
    /// <returns>An optional entity that contains both specified component types, or none if no such entity exists.</returns>
    public Option<Entity> QueryFirst<T1, T2>() where T1 : struct where T2 : struct
    {
        var s1 = GetStore<T1>();
        var s2 = GetStore<T2>();
        if (s1.Count <= s2.Count)
        {
            for (var i = 0; i < s1.Count; i++) {
                var id = s1.GetEntityIdAt(i);
                if (s2.Has(id)) return Option<Entity>.Some(new Entity(id));
            }
        } else {
            for (var i = 0; i < s2.Count; i++) {
                var id = s2.GetEntityIdAt(i);
                if (s1.Has(id)) return Option<Entity>.Some(new Entity(id));
            }
        }
        return Option<Entity>.None;
    }

    /// <summary>
    /// Queries for the first entity that contains all specified component types.
    /// </summary>
    /// <typeparam name="T1">The first component type to query for.</typeparam>
    /// <typeparam name="T2">The second component type to query for.</typeparam>
    /// <typeparam name="T3">The third component type to query for.</typeparam>
    /// <returns>An optional entity that contains all the specified component types, or none if no such entity exists.</returns>
    public Option<Entity> QueryFirst<T1, T2, T3>() where T1 : struct where T2 : struct where T3 : struct
    {
        var s1 = GetStore<T1>();
        var s2 = GetStore<T2>();
        var s3 = GetStore<T3>();
        var min = Math.Min(s1.Count, Math.Min(s2.Count, s3.Count));
    
        if (min == s1.Count) {
            for (var i = 0; i < s1.Count; i++) {
                var id = s1.GetEntityIdAt(i);
                if (s2.Has(id) && s3.Has(id)) return Option<Entity>.Some(new Entity(id));
            }
        } else if (min == s2.Count) {
            for (var i = 0; i < s2.Count; i++) {
                var id = s2.GetEntityIdAt(i);
                if (s1.Has(id) && s3.Has(id)) return Option<Entity>.Some(new Entity(id));
            }
        } else {
            for (var i = 0; i < s3.Count; i++) {
                var id = s3.GetEntityIdAt(i);
                if (s1.Has(id) && s2.Has(id)) return Option<Entity>.Some(new Entity(id));
            }
        }
        return Option<Entity>.None;
    }
    
    /// <summary>
    /// Queries all entities in the world containing the specified component type.
    /// </summary>
    /// <typeparam name="T">The type of the component to query for.</typeparam>
    /// <returns>An enumerable of entities that contain the specified component type.</returns>
    public IEnumerable<Entity> Query<T>() where T : struct
    {
        var store = GetStore<T>();
        for (var i = 0; i < store.Count; i++)
            yield return new Entity(store.GetEntityIdAt(i));
    }

    /// <summary>
    /// Retrieves an enumerable collection of entities that contain the specified two components.
    /// </summary>
    /// <typeparam name="T1">The first component type to query for.</typeparam>
    /// <typeparam name="T2">The second component type to query for.</typeparam>
    /// <returns>An enumerable collection of entities containing both specified components.</returns>
    public IEnumerable<Entity> Query<T1, T2>() where T1 : struct where T2 : struct
    {
        var s1 = GetStore<T1>();
        var s2 = GetStore<T2>();

        if (s1.Count <= s2.Count)
        {
            for (var i = 0; i < s1.Count; i++)
            {
                var id = s1.GetEntityIdAt(i);
                if (s2.Has(id)) yield return new Entity(id);
            }
        }
        else
        {
            for (var i = 0; i < s2.Count; i++)
            {
                var id = s2.GetEntityIdAt(i);
                if (s1.Has(id)) yield return new Entity(id);
            }
        }
    }

    /// <summary>
    /// Queries the world for entities that contain the specified three component types.
    /// </summary>
    /// <typeparam name="T1">The first component type to match.</typeparam>
    /// <typeparam name="T2">The second component type to match.</typeparam>
    /// <typeparam name="T3">The third component type to match.</typeparam>
    /// <returns>An enumerable of entities containing all the specified component types.</returns>
    public IEnumerable<Entity> Query<T1, T2, T3>() where T1 : struct where T2 : struct where T3 : struct
    {
        var s1 = GetStore<T1>();
        var s2 = GetStore<T2>();
        var s3 = GetStore<T3>();
        var min = Math.Min(s1.Count, Math.Min(s2.Count, s3.Count));

        if (min == s1.Count) {
            for (var i = 0; i < s1.Count; i++) {
                var id = s1.GetEntityIdAt(i);
                if (s2.Has(id) && s3.Has(id)) yield return new Entity(id);
            }
        } else if (min == s2.Count) {
            for (var i = 0; i < s2.Count; i++) {
                var id = s2.GetEntityIdAt(i);
                if (s1.Has(id) && s3.Has(id)) yield return new Entity(id);
            }
        } else {
            for (var i = 0; i < s3.Count; i++) {
                var id = s3.GetEntityIdAt(i);
                if (s1.Has(id) && s2.Has(id)) yield return new Entity(id);
            }
        }
    }

    /// <summary>
    /// Gets all components of a specific type with their associated entities.
    /// </summary>
    /// <typeparam name="T">The type of the component.</typeparam>
    /// <returns>An enumerable of tuples containing the entity and its component.</returns>
    public IEnumerable<T> QueryWith<T>() where T : struct
    {
        var s = GetStore<T>();
        for (var i = 0; i < s.Count; i++) {
            yield return s.GetByIndex(i);
        }
    }

    /// <summary>
    /// Queries all entities that have the specified components and retrieves the entities paired with the associated components.
    /// </summary>
    /// <typeparam name="T1">The type of the first component to query.</typeparam>
    /// <typeparam name="T2">The type of the second component to query.</typeparam>
    /// <returns>An enumerable of tuples containing the entity and its associated components.</returns>
    public IEnumerable<(T1 c1, T2 c2)> QueryWith<T1, T2>() where T1 : struct where T2 : struct
    {
        var s1 = GetStore<T1>();
        var s2 = GetStore<T2>();

        if (s1.Count <= s2.Count) {
            for (var i = 0; i < s1.Count; i++) {
                var id = s1.GetEntityIdAt(i);
                if (s2.Has(id)) yield return (s1.GetByIndex(i), s2.Get(id));
            }
        } else {
            for (var i = 0; i < s2.Count; i++) {
                var id = s2.GetEntityIdAt(i);
                if (s1.Has(id)) yield return (s1.Get(id), s2.GetByIndex(i));
            }
        }
    }

    /// <summary>
    /// Queries the world for entities that have components of the specified types and retrieves the components for each entity as a tuple.
    /// </summary>
    /// <typeparam name="T1">The type of the first component to query.</typeparam>
    /// <typeparam name="T2">The type of the second component to query.</typeparam>
    /// <typeparam name="T3">The type of the third component to query.</typeparam>
    /// <returns>An enumerable of tuples where each tuple contains components of type <typeparamref name="T1"/>, <typeparamref name="T2"/>, and <typeparamref name="T3"/> for a single entity.</returns>
    public IEnumerable<(T1 c1, T2 c2, T3 c3)> QueryWith<T1, T2, T3>()
        where T1 : struct where T2 : struct where T3 : struct
    {
        var s1 = GetStore<T1>();
        var s2 = GetStore<T2>();
        var s3 = GetStore<T3>();
        var min = Math.Min(s1.Count, Math.Min(s2.Count, s3.Count));

        if (min == s1.Count) {
            for (var i = 0; i < s1.Count; i++) {
                var id = s1.GetEntityIdAt(i);
                if (s2.Has(id) && s3.Has(id)) yield return (s1.GetByIndex(i), s2.Get(id), s3.Get(id));
            }
        } else if (min == s2.Count) {
            for (var i = 0; i < s2.Count; i++) {
                var id = s2.GetEntityIdAt(i);
                if (s1.Has(id) && s3.Has(id)) yield return (s1.Get(id), s2.GetByIndex(i), s3.Get(id));
            }
        } else {
            for (var i = 0; i < s3.Count; i++) {
                var id = s3.GetEntityIdAt(i);
                if (s1.Has(id) && s2.Has(id)) yield return (s1.Get(id), s2.Get(id), s3.GetByIndex(i));
            }
        }
    }

    /// <summary>
    /// Retrieves the component store of the specified type, creating it if it does not exist.
    /// </summary>
    /// <typeparam name="T">The type of the components stored in the component store.</typeparam>
    /// <returns>The component store of the specified type.</returns>
    public ComponentStore<T> GetStore<T>() where T : struct
    {
        var type = typeof(T);
        if (_stores.TryGetValue(type, out var store)) return (ComponentStore<T>)store;
        store = new ComponentStore<T>();
        _stores[type] = store;
        return (ComponentStore<T>)store;
    }
}

/// <summary>
/// Unit type for Result types that don't need to return a value.
/// </summary>
public readonly struct Unit
{
    public static Unit Default => default;
}

/// <summary>
/// Represents a storage mechanism for managing components in the Entity Component System.
/// Provides operations for removing components associated with specific entities.
/// </summary>
internal interface IComponentStore
{
    void Remove(int entityId);
}

/// <summary>
/// Represents a strongly-typed storage for components used in the Entity Component System.
/// Manages associations between components of a specific type and their entity identifiers, providing efficient storage, retrieval, and removal operations.
/// </summary>
/// <typeparam name="T">The type of components stored in this component store. Must be a value type.</typeparam>
public class ComponentStore<T> : IComponentStore where T : struct
{
    private T[] _instances = new T[1024];
    private int[] _sparse = new int[4096]; // Index = EntityId
    private int[] _dense = new int[1024];  // Index = Position dans _instances

    public ComponentStore() => Array.Fill(_sparse, -1);

    public void Add(int entityId, T component)
    {
        if (entityId >= _sparse.Length) Array.Resize(ref _sparse, entityId * 2);
        if (Count >= _instances.Length) {
            Array.Resize(ref _instances, Count * 2);
            Array.Resize(ref _dense, Count * 2);
        }

        var index = Count++;
        _instances[index] = component;
        _dense[index] = entityId;
        _sparse[entityId] = index;
    }

    public ref T Get(int entityId) => ref _instances[_sparse[entityId]];
    public bool Has(int entityId) => entityId < _sparse.Length && _sparse[entityId] != -1;
    
    public void Remove(int entityId) {
        if (!Has(entityId)) return;
        var idx = _sparse[entityId];
        _instances[idx] = _instances[--Count]; // Swap back
        _dense[idx] = _dense[Count];
        _sparse[_dense[idx]] = idx;
        _sparse[entityId] = -1;
    }

    public int Count { get; private set; }
    public ref T GetByIndex(int index) => ref _instances[index];
    public int GetEntityIdAt(int index) => _dense[index];
    
}
