using System;
using System.Collections.Generic;
using IronStrata.Scripts.Core.Types;

namespace IronStrata.Scripts.Core.ECS;

/// <summary>
/// Represents the core World class for managing entities and components in an Entity Component System (ECS).
/// </summary>
/// <remarks>
/// This class provides functionalities for creating and destroying entities, managing their components,
/// querying and iterating over entities and components, and handling events related to entity lifecycle.
/// </remarks>
public class World {
    /// <summary>
    /// Tracks the next available unique identifier for newly created entities.
    /// This ensures that each entity within the system is assigned a distinct ID, avoiding conflicts
    /// and maintaining the integrity of entity references during their lifecycle.
    /// </summary>
    private int _nextId;

    /// <summary>
    /// Maintains a collection of IDs representing currently active and valid entities within the system.
    /// This ensures efficient tracking of entity lifecycles and facilitates operations such as creation, destruction, and queries.
    /// </summary>
    private readonly HashSet<int> _alive = [];

    /// <summary>
    /// Serves as a pool for recycling entity IDs that are no longer in use,
    /// enabling reuse of IDs to optimize resource management and reduce fragmentation within the system.
    /// </summary>
    private readonly Queue<int> _recycled = new();

    /// <summary>
    /// Maintains a mapping of component types to their corresponding component stores,
    /// enabling efficient storage and retrieval of components for entities within the framework.
    /// </summary>
    private readonly Dictionary<Type, IComponentStore> _stores = [];

    /// <summary>
    /// Event triggered whenever a new entity is created in the world.
    /// Subscribers to this event are notified with the newly created entity,
    /// allowing them to perform initialization or listen for changes associated
    /// with the entity's lifecycle.
    /// </summary>
    public event Action<Entity> OnEntityCreated;

    /// <summary>
    /// Invoked whenever an entity is destroyed within the ECS system.
    /// This event allows systems and components to respond to the removal of entities,
    /// enabling cleanup operations, state updates, or any necessary reactions to the entity's destruction.
    /// </summary>
    public event Action<Entity> OnEntityDestroyed;

    /// <summary>
    /// Creates a new entity in the world and initializes it with a unique ID.
    /// </summary>
    /// <returns>The newly created entity.</returns>
    public Entity CreateEntity() {
        int id = _recycled.Count > 0 ? _recycled.Dequeue() : _nextId++;
        _alive.Add(id);
        Entity entity = new(id);
        OnEntityCreated?.Invoke(entity);
        return entity;
    }

    /// <summary>
    /// Destroys the specified entity, removing it from the world, deallocating its components,
    /// and marking its ID for reuse.
    /// </summary>
    /// <param name="entity">The entity to destroy.</param>
    public void DestroyEntity(Entity entity) {
        if (!_alive.Contains(entity.Id)) {
            return;
        }

        foreach (IComponentStore store in _stores.Values) {
            store.Remove(entity.Id);
        }

        _alive.Remove(entity.Id);
        _recycled.Enqueue(entity.Id);
        OnEntityDestroyed?.Invoke(entity);
    }

    /// <summary>
    /// Safely destroys an entity if it exists.
    /// </summary>
    /// <param name="entity">The entity to destroy.</param>
    /// <returns>A Result indicating success or failure with reason.</returns>
    public Result<Unit, string> TryDestroyEntity(Entity entity) {
        if (!_alive.Contains(entity.Id)) {
            return Result<Unit, string>.Err($"Entity {entity.Id} is not alive");
        }

        DestroyEntity(entity);
        return Result<Unit, string>.Ok(default);
    }

    /// <summary>
    /// Determines whether the specified entity is currently active and managed by the world.
    /// </summary>
    /// <param name="entity">The entity to check.</param>
    /// <returns>True if the entity is alive; otherwise, false.</returns>
    public bool IsAlive(Entity entity) => _alive.Contains(entity.Id);

    /// <summary>
    /// Represents the total count of currently active entities in the system.
    /// This value reflects the number of entities that are alive and managed
    /// within the world at a given time.
    /// </summary>
    public int EntityCount => _alive.Count;

    /// <summary>
    /// Adds or updates a component for an entity.
    /// </summary>
    /// <typeparam name="T">The type of the component.</typeparam>
    /// <param name="entity">The target entity.</param>
    /// <param name="component">The component instance to add.</param>
    public void Add<T>(Entity entity, T component) where T : struct => GetStore<T>().Add(entity.Id, component);

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
    public Result<Entity, string> SafeAdd<T>(Entity entity, T component) where T : struct {
        if (entity.IsNull) {
            return Result<Entity, string>.Err("L'entité est nulle");
        }

        Add(entity, component);
        return Result<Entity, string>.Ok(entity);
    }

    /// <summary>
    /// Retrieves a component from an entity as an Option.
    /// </summary>
    /// <typeparam name="T">The type of the component.</typeparam>
    /// <returns>An Option containing the component if found, or None otherwise.</returns>
    public Option<T> GetOptional<T>(Entity entity) where T : struct {
        ComponentStore<T> store = GetStore<T>();
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
    public Option<T> TryGet<T>(Entity entity) where T : struct {
        ComponentStore<T> store = GetStore<T>();
        return store.Has(entity.Id) ? Option<T>.Some(store.Get(entity.Id)) : Option<T>.None;
    }

    /// <summary>
    /// Checks whether the specified entity has a component of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the component to check for.</typeparam>
    /// <param name="entity">The entity to check.</param>
    /// <returns>True if the entity contains a component of the specified type; otherwise, false.</returns>
    public bool Has<T>(Entity entity) where T : struct => GetStore<T>().Has(entity.Id);

    /// <summary>
    /// Removes the component of the specified type from the given entity.
    /// </summary>
    /// <typeparam name="T">The type of the component to remove.</typeparam>
    /// <param name="entity">The entity from which the component will be removed.</param>
    public void Remove<T>(Entity entity) where T : struct {
        if (_stores.TryGetValue(typeof(T), out IComponentStore store)) {
            store.Remove(entity.Id);
        }
    }

    /// <summary>
    /// Removes a specific component from an entity and returns it if it existed.
    /// </summary>
    /// <typeparam name="T">The type of the component.</typeparam>
    /// <param name="entity">The target entity.</param>
    /// <returns>An Option containing the removed component if it existed.</returns>
    public Option<T> RemoveAndGet<T>(Entity entity) where T : struct {
        Option<T> component = GetOptional<T>(entity);
        Remove<T>(entity);
        return component;
    }


    /// <summary>
    /// Represents an action that operates on a single component within the ECS.
    /// </summary>
    /// <typeparam name="T1">The type of the component.</typeparam>
    public delegate void QueryAction<T1>(ref T1 component1);

    /// <summary>
    /// Represents an action that operates on pairs of components of specified types within the ECS.
    /// Used in queries to process entities that have both components.
    /// </summary>
    /// <typeparam name="T1">The type of the first component in the pair.</typeparam>
    /// <typeparam name="T2">The type of the second component in the pair.</typeparam>
    public delegate void QueryAction<T1, T2>(ref T1 component1, ref T2 component2);

    /// <summary>
    /// Represents an action that operates on three components of specified types within the ECS.
    /// </summary>
    /// <typeparam name="T1">The type of the first component.</typeparam>
    /// <typeparam name="T2">The type of the second component.</typeparam>
    /// <typeparam name="T3">The type of the third component.</typeparam>
    public delegate void QueryAction<T1, T2, T3>(ref T1 component1, ref T2 component2, ref T3 component3);

    /// <summary>
    /// Represents a delegate that defines an action to be performed on an entity and a reference to its component of type <typeparamref name="T1"/>.
    /// </summary>
    /// <typeparam name="T1">The type of the component associated with the entity. Must be a value type.</typeparam>
    /// <param name="entity">The entity on which the action is being performed.</param>
    /// <param name="component1">A reference to the component of type <typeparamref name="T1"/> associated with the entity.</param>
    /// <remarks>
    /// This delegate is used in the context of executing entity-component actions within a world,
    /// allowing operations that require both an entity reference and a component reference.
    /// </remarks>
    public delegate void EntityQueryAction<T1>(Entity entity, ref T1 component1);

    /// <summary>
    /// Represents a delegate for performing actions on an entity and its associated components during a query.
    /// </summary>
    /// <typeparam name="T1">The type of the first component associated with the entity.</typeparam>
    /// <typeparam name="T2">The type of the second component associated with the entity.</typeparam>
    /// <remarks>
    /// This delegate is used in queries to handle entities and their corresponding components directly,
    /// enabling custom logic to be applied to each entity and its components during iteration.
    /// </remarks>
    public delegate void EntityQueryAction<T1, T2>(Entity entity, ref T1 component1, ref T2 component2);

    /// <summary>
    /// Represents a delegate that operates on an entity and its associated components.
    /// </summary>
    /// <typeparam name="T1">The type of the first component associated with the entity.</typeparam>
    /// <typeparam name="T2">The type of the second component associated with the entity.</typeparam>
    /// <typeparam name="T3">The type of the third component associated with the entity.</typeparam>
    /// <remarks>
    /// This delegate is used for defining actions that involve an <see cref="Entity"/>
    /// along with three of its components, allowing for modification or inspection of their state.
    /// </remarks>
    public delegate void EntityQueryAction<T1, T2, T3>(Entity entity, ref T1 component1, ref T2 component2, ref T3 component3);

    /// <summary>
    /// Iterates over all components of type <typeparamref name="T1"/> in the world and executes the specified action for each component.
    /// </summary>
    /// <typeparam name="T1">The type of the component to iterate over.</typeparam>
    /// <param name="action">
    /// A delegate that defines the action to be performed on each component.
    /// The action receives a reference to the current component of type <typeparamref name="T1"/>.
    /// </param>
    public void ForEach<T1>(QueryAction<T1> action) where T1 : struct {
        ComponentStore<T1> store = GetStore<T1>();
        for (int index = 0; index < store.Count; index++) {
            action(ref store.GetByIndex(index));
        }
    }

    /// <summary>
    /// Executes a specified action on all entities in the world that have a component of type <typeparamref name="T1"/>.
    /// </summary>
    /// <typeparam name="T1">The type of the component to query.</typeparam>
    /// <param name="action">
    /// The action to be executed for each matching entity. The action receives the entity and a reference to its associated component.
    /// </param>
    public void ForEach<T1>(EntityQueryAction<T1> action) where T1 : struct {
        ComponentStore<T1> store = GetStore<T1>();
        for (int index = 0; index < store.Count; index++) {
            action(new Entity(store.GetEntityIdAt(index)), ref store.GetByIndex(index));
        }
    }

    /// <summary>
    /// Executes an action for all entities that contain both component types T1 and T2.
    /// </summary>
    /// <param name="action">The action to execute for each matching entity. Provides references to the components of type T1 and T2.</param>
    /// <typeparam name="T1">The type of the first component.</typeparam>
    /// <typeparam name="T2">The type of the second component.</typeparam>
    public void ForEach<T1, T2>(QueryAction<T1, T2> action)
        where T1 : struct
        where T2 : struct {
        ComponentStore<T1> store1 = GetStore<T1>();
        ComponentStore<T2> store2 = GetStore<T2>();

        if (store1.Count <= store2.Count) {
            for (int index = 0; index < store1.Count; index++) {
                int entityId = store1.GetEntityIdAt(index);
                if (store2.Has(entityId)) {
                    action(ref store1.GetByIndex(index), ref store2.Get(entityId));
                }
            }
        }
        else {
            for (int index = 0; index < store2.Count; index++) {
                int entityId = store2.GetEntityIdAt(index);
                if (store1.Has(entityId)) {
                    action(ref store1.Get(entityId), ref store2.GetByIndex(index));
                }
            }
        }
    }

    /// <summary>
    /// Iterates through entities that have the specified components and invokes the provided action for each entity.
    /// </summary>
    /// <typeparam name="T1">The type of the first component.</typeparam>
    /// <typeparam name="T2">The type of the second component.</typeparam>
    /// <param name="action">
    /// The action to be performed on each entity. It receives the entity, a reference to the first component,
    /// and a reference to the second component.
    /// </param>
    public void ForEach<T1, T2>(EntityQueryAction<T1, T2> action)
        where T1 : struct
        where T2 : struct {
        ComponentStore<T1> store1 = GetStore<T1>();
        ComponentStore<T2> store2 = GetStore<T2>();

        if (store1.Count <= store2.Count) {
            for (int index = 0; index < store1.Count; index++) {
                int entityId = store1.GetEntityIdAt(index);
                if (store2.Has(entityId)) {
                    action(new Entity(entityId), ref store1.GetByIndex(index), ref store2.Get(entityId));
                }
            }
        }
        else {
            for (int index = 0; index < store2.Count; index++) {
                int entityId = store2.GetEntityIdAt(index);
                if (store1.Has(entityId)) {
                    action(new Entity(entityId), ref store1.Get(entityId), ref store2.GetByIndex(index));
                }
            }
        }
    }

    /// <summary>
    /// Iterates over all entities that have components of types <typeparamref name="T1"/>, <typeparamref name="T2"/>, and <typeparamref name="T3"/>,
    /// and invokes the specified action for each combination of components.
    /// </summary>
    /// <typeparam name="T1">The type of the first component.</typeparam>
    /// <typeparam name="T2">The type of the second component.</typeparam>
    /// <typeparam name="T3">The type of the third component.</typeparam>
    /// <param name="action">The action to perform for each set of components. Receives references to the components of each entity.</param>
    public void ForEach<T1, T2, T3>(QueryAction<T1, T2, T3> action)
        where T1 : struct where T2 : struct where T3 : struct {
        ComponentStore<T1> store1 = GetStore<T1>();
        ComponentStore<T2> store2 = GetStore<T2>();
        ComponentStore<T3> store3 = GetStore<T3>();
        int minimumCount = Math.Min(store1.Count, Math.Min(store2.Count, store3.Count));

        if (minimumCount == store1.Count) {
            for (int index = 0; index < store1.Count; index++) {
                int entityId = store1.GetEntityIdAt(index);
                if (store2.Has(entityId) && store3.Has(entityId)) {
                    action(ref store1.GetByIndex(index), ref store2.Get(entityId), ref store3.Get(entityId));
                }
            }
        }
        else if (minimumCount == store2.Count) {
            for (int index = 0; index < store2.Count; index++) {
                int entityId = store2.GetEntityIdAt(index);
                if (store1.Has(entityId) && store3.Has(entityId)) {
                    action(ref store1.Get(entityId), ref store2.GetByIndex(index), ref store3.Get(entityId));
                }
            }
        }
        else {
            for (int index = 0; index < store3.Count; index++) {
                int entityId = store3.GetEntityIdAt(index);
                if (store1.Has(entityId) && store2.Has(entityId)) {
                    action(ref store1.Get(entityId), ref store2.Get(entityId), ref store3.GetByIndex(index));
                }
            }
        }
    }

    /// <summary>
    /// Iterates over all entities that have the specified component types and invokes the given action for each entity.
    /// </summary>
    /// <typeparam name="T1">The type of the first component.</typeparam>
    /// <typeparam name="T2">The type of the second component.</typeparam>
    /// <typeparam name="T3">The type of the third component.</typeparam>
    /// <param name="action">
    /// The action to execute for each matching entity, providing the entity and references to its components of type T1, T2, and T3.
    /// </param>
    public void ForEach<T1, T2, T3>(EntityQueryAction<T1, T2, T3> action)
        where T1 : struct where T2 : struct where T3 : struct {
        ComponentStore<T1> store1 = GetStore<T1>();
        ComponentStore<T2> store2 = GetStore<T2>();
        ComponentStore<T3> store3 = GetStore<T3>();
        int minimumCount = Math.Min(store1.Count, Math.Min(store2.Count, store3.Count));

        if (minimumCount == store1.Count) {
            for (int index = 0; index < store1.Count; index++) {
                int entityId = store1.GetEntityIdAt(index);
                if (store2.Has(entityId) && store3.Has(entityId)) {
                    action(new Entity(entityId), ref store1.GetByIndex(index), ref store2.Get(entityId), ref store3.Get(entityId));
                }
            }
        }
        else if (minimumCount == store2.Count) {
            for (int index = 0; index < store2.Count; index++) {
                int entityId = store2.GetEntityIdAt(index);
                if (store1.Has(entityId) && store3.Has(entityId)) {
                    action(new Entity(entityId), ref store1.Get(entityId), ref store2.GetByIndex(index), ref store3.Get(entityId));
                }
            }
        }
        else {
            for (int index = 0; index < store3.Count; index++) {
                int entityId = store3.GetEntityIdAt(index);
                if (store1.Has(entityId) && store2.Has(entityId)) {
                    action(new Entity(entityId), ref store1.Get(entityId), ref store2.Get(entityId), ref store3.GetByIndex(index));
                }
            }
        }
    }

    /// <summary>
    /// Queries the world for the first entity containing the specified component type.
    /// </summary>
    /// <typeparam name="T">The type of the component to query for.</typeparam>
    /// <returns>An option containing the first entity with the specified component type, or none if no such entity exists.</returns>
    public Option<Entity> QueryFirst<T>() where T : struct {
        ComponentStore<T> store = GetStore<T>();
        foreach (int entityId in _alive) {
            if (store.Has(entityId)) {
                return Option<Entity>.Some(new Entity(entityId));
            }
        }

        return Option<Entity>.None;
    }


    /// <summary>
    ///  Queries the world for the first entity containing the specified component types.
    /// </summary>
    /// <typeparam name="T1">The first component type to query for.</typeparam>
    /// <typeparam name="T2">The second component type to query for.</typeparam>
    /// <returns>An optional entity that contains both specified component types, or none if no such entity exists.</returns>
    public Option<Entity> QueryFirst<T1, T2>() where T1 : struct where T2 : struct {
        ComponentStore<T1> store1 = GetStore<T1>();
        ComponentStore<T2> store2 = GetStore<T2>();
        if (store1.Count <= store2.Count) {
            for (int index = 0; index < store1.Count; index++) {
                int entityId = store1.GetEntityIdAt(index);
                if (store2.Has(entityId)) {
                    return Option<Entity>.Some(new Entity(entityId));
                }
            }
        }
        else {
            for (int index = 0; index < store2.Count; index++) {
                int entityId = store2.GetEntityIdAt(index);
                if (store1.Has(entityId)) {
                    return Option<Entity>.Some(new Entity(entityId));
                }
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
    public Option<Entity> QueryFirst<T1, T2, T3>() where T1 : struct where T2 : struct where T3 : struct {
        ComponentStore<T1> store1 = GetStore<T1>();
        ComponentStore<T2> store2 = GetStore<T2>();
        ComponentStore<T3> store3 = GetStore<T3>();
        int minimumCount = Math.Min(store1.Count, Math.Min(store2.Count, store3.Count));

        if (minimumCount == store1.Count) {
            for (int index = 0; index < store1.Count; index++) {
                int entityId = store1.GetEntityIdAt(index);
                if (store2.Has(entityId) && store3.Has(entityId)) {
                    return Option<Entity>.Some(new Entity(entityId));
                }
            }
        }
        else if (minimumCount == store2.Count) {
            for (int index = 0; index < store2.Count; index++) {
                int entityId = store2.GetEntityIdAt(index);
                if (store1.Has(entityId) && store3.Has(entityId)) {
                    return Option<Entity>.Some(new Entity(entityId));
                }
            }
        }
        else {
            for (int index = 0; index < store3.Count; index++) {
                int entityId = store3.GetEntityIdAt(index);
                if (store1.Has(entityId) && store2.Has(entityId)) {
                    return Option<Entity>.Some(new Entity(entityId));
                }
            }
        }

        return Option<Entity>.None;
    }

    /// <summary>
    /// Queries all entities in the world containing the specified component type.
    /// </summary>
    /// <typeparam name="T">The type of the component to query for.</typeparam>
    /// <returns>An enumerable of entities that contain the specified component type.</returns>
    public IEnumerable<Entity> Query<T>() where T : struct {
        ComponentStore<T> store = GetStore<T>();
        for (int index = 0; index < store.Count; index++) {
            yield return new Entity(store.GetEntityIdAt(index));
        }
    }

    /// <summary>
    /// Retrieves an enumerable collection of entities that contain the specified two components.
    /// </summary>
    /// <typeparam name="T1">The first component type to query for.</typeparam>
    /// <typeparam name="T2">The second component type to query for.</typeparam>
    /// <returns>An enumerable collection of entities containing both specified components.</returns>
    public IEnumerable<Entity> Query<T1, T2>() where T1 : struct where T2 : struct {
        ComponentStore<T1> store1 = GetStore<T1>();
        ComponentStore<T2> store2 = GetStore<T2>();

        if (store1.Count <= store2.Count) {
            for (int index = 0; index < store1.Count; index++) {
                int entityId = store1.GetEntityIdAt(index);
                if (store2.Has(entityId)) {
                    yield return new Entity(entityId);
                }
            }
        }
        else {
            for (int index = 0; index < store2.Count; index++) {
                int entityId = store2.GetEntityIdAt(index);
                if (store1.Has(entityId)) {
                    yield return new Entity(entityId);
                }
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
    public IEnumerable<Entity> Query<T1, T2, T3>() where T1 : struct where T2 : struct where T3 : struct {
        ComponentStore<T1> store1 = GetStore<T1>();
        ComponentStore<T2> store2 = GetStore<T2>();
        ComponentStore<T3> store3 = GetStore<T3>();
        int minimumCount = Math.Min(store1.Count, Math.Min(store2.Count, store3.Count));

        if (minimumCount == store1.Count) {
            for (int index = 0; index < store1.Count; index++) {
                int entityId = store1.GetEntityIdAt(index);
                if (store2.Has(entityId) && store3.Has(entityId)) {
                    yield return new Entity(entityId);
                }
            }
        }
        else if (minimumCount == store2.Count) {
            for (int index = 0; index < store2.Count; index++) {
                int entityId = store2.GetEntityIdAt(index);
                if (store1.Has(entityId) && store3.Has(entityId)) {
                    yield return new Entity(entityId);
                }
            }
        }
        else {
            for (int index = 0; index < store3.Count; index++) {
                int entityId = store3.GetEntityIdAt(index);
                if (store1.Has(entityId) && store2.Has(entityId)) {
                    yield return new Entity(entityId);
                }
            }
        }
    }

    /// <summary>
    /// Gets all components of a specific type with their associated entities.
    /// </summary>
    /// <typeparam name="T">The type of the component.</typeparam>
    /// <returns>An enumerable of tuples containing the entity and its component.</returns>
    public IEnumerable<T> QueryWith<T>() where T : struct {
        ComponentStore<T> store = GetStore<T>();
        for (int index = 0; index < store.Count; index++) { yield return store.GetByIndex(index); }
    }

    /// <summary>
    /// Queries all entities that have the specified components and retrieves the entities paired with the associated components.
    /// </summary>
    /// <typeparam name="T1">The type of the first component to query.</typeparam>
    /// <typeparam name="T2">The type of the second component to query.</typeparam>
    /// <returns>An enumerable of tuples containing the entity and its associated components.</returns>
    public IEnumerable<(T1 c1, T2 c2)> QueryWith<T1, T2>() where T1 : struct where T2 : struct {
        ComponentStore<T1> store1 = GetStore<T1>();
        ComponentStore<T2> store2 = GetStore<T2>();

        if (store1.Count <= store2.Count) {
            for (int index = 0; index < store1.Count; index++) {
                int entityId = store1.GetEntityIdAt(index);
                if (store2.Has(entityId)) {
                    yield return (store1.GetByIndex(index), store2.Get(entityId));
                }
            }
        }
        else {
            for (int index = 0; index < store2.Count; index++) {
                int entityId = store2.GetEntityIdAt(index);
                if (store1.Has(entityId)) {
                    yield return (store1.Get(entityId), store2.GetByIndex(index));
                }
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
        where T1 : struct where T2 : struct where T3 : struct {
        ComponentStore<T1> store1 = GetStore<T1>();
        ComponentStore<T2> store2 = GetStore<T2>();
        ComponentStore<T3> store3 = GetStore<T3>();
        int minimumCount = Math.Min(store1.Count, Math.Min(store2.Count, store3.Count));

        if (minimumCount == store1.Count) {
            for (int index = 0; index < store1.Count; index++) {
                int entityId = store1.GetEntityIdAt(index);
                if (store2.Has(entityId) && store3.Has(entityId)) {
                    yield return (store1.GetByIndex(index), store2.Get(entityId), store3.Get(entityId));
                }
            }
        }
        else if (minimumCount == store2.Count) {
            for (int index = 0; index < store2.Count; index++) {
                int entityId = store2.GetEntityIdAt(index);
                if (store1.Has(entityId) && store3.Has(entityId)) {
                    yield return (store1.Get(entityId), store2.GetByIndex(index), store3.Get(entityId));
                }
            }
        }
        else {
            for (int index = 0; index < store3.Count; index++) {
                int entityId = store3.GetEntityIdAt(index);
                if (store1.Has(entityId) && store2.Has(entityId)) {
                    yield return (store1.Get(entityId), store2.Get(entityId), store3.GetByIndex(index));
                }
            }
        }
    }

    /// <summary>
    /// Retrieves the component store of the specified type, creating it if it does not exist.
    /// </summary>
    /// <typeparam name="T">The type of the components stored in the component store.</typeparam>
    /// <returns>The component store of the specified type.</returns>
    public ComponentStore<T> GetStore<T>() where T : struct {
        Type type = typeof(T);
        if (_stores.TryGetValue(type, out IComponentStore store)) {
            return (ComponentStore<T>)store;
        }

        store = new ComponentStore<T>();
        _stores[type] = store;
        return (ComponentStore<T>)store;
    }
}

/// <summary>
/// Represents a unit type with no meaningful data, often used as a placeholder or a signal in the context of functional programming.
/// </summary>
/// <remarks>
/// This struct is used in scenarios where a result or a return value is required but no actual data needs to be carried.
/// It serves as a type-safe alternative to using void in contexts where a value is expected, such as with generic result types.
/// </remarks>
public readonly struct Unit {
    public static Unit Default => default;
}

/// <summary>
/// Represents a storage mechanism for managing components in the Entity Component System.
/// Provides operations for removing components associated with specific entities.
/// </summary>
internal interface IComponentStore {
    void Remove(int entityId);
}

/// <summary>
/// Represents a strongly-typed storage for components used in the Entity Component System.
/// Manages associations between components of a specific type and their entity identifiers, providing efficient storage, retrieval, and removal operations.
/// </summary>
/// <typeparam name="T">The type of components stored in this component store. Must be a value type.</typeparam>
public class ComponentStore<T> : IComponentStore where T : struct {
    /// <summary>
    /// Stores an array of components associated with entities in the Entity Component System.
    /// This array serves as the primary storage for components of a specific type, enabling efficient
    /// addition, retrieval, and manipulation of components for registered entities.
    /// </summary>
    private T[] _instances = new T[1024];

    /// <summary>
    /// Maps entity identifiers to their corresponding indices within the dense array
    /// in the Entity Component System's component storage. This provides efficient
    /// lookup for determining the position of an entity's component within the underlying
    /// data structure. Unused or removed entity identifiers are marked with a sentinel value
    /// to indicate their absence.
    /// </summary>
    private int[] _sparse = new int[4096];

    /// <summary>
    /// Maintains a dense array of entity identifiers, mapping component storage indices to the corresponding entities.
    /// This array allows for efficient iteration over active component-entity associations while enabling quick lookups
    /// and updates during operations such as addition, removal, and retrieval of components.
    /// </summary>
    private int[] _dense = new int[1024];

    /// <summary>
    /// Provides a strongly-typed storage mechanism for managing components of a specific type in an Entity Component System.
    /// Allows efficient operations such as adding, retrieving, checking, and removing components associated with specific entity IDs.
    /// </summary>
    /// <typeparam name="T">The type of components to be stored. Must be a value type.</typeparam>
    public ComponentStore() {
        Array.Fill(_sparse, -1);
    }

    /// <summary>
    /// Adds a component of the specified type to the given entity.
    /// </summary>
    /// <typeparam name="T">The type of the component to add. Must be a value type.</typeparam>
    /// <param name="entityId">The entity to which the component will be added.</param>
    /// <param name="component">The component instance to add to the entity.</param>
    public void Add(int entityId, T component) {
        if (entityId >= _sparse.Length) {
            Array.Resize(ref _sparse, entityId * 2);
        }

        if (Count >= _instances.Length) {
            Array.Resize(ref _instances, Count * 2);
            Array.Resize(ref _dense, Count * 2);
        }

        int index = Count++;
        _instances[index] = component;
        _dense[index] = entityId;
        _sparse[entityId] = index;
    }

    /// <summary>
    /// Retrieves a reference to the component of the specified type associated with the given entity.
    /// </summary>
    /// <typeparam name="T">The type of the component to retrieve. Must be a value type.</typeparam>
    /// <param name="entityId">The entity for which the component is to be retrieved.</param>
    /// <returns>A reference to the component associated with the specified entity.</returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the entity does not have a component of the specified type.
    /// </exception>
    public ref T Get(int entityId) => ref _instances[_sparse[entityId]];

    /// <summary>
    /// Determines whether the specified entity has a component of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of component to check for. Must be a value type.</typeparam>
    /// <param name="entityId">The entity to check for the presence of the component.</param>
    /// <returns>True if the entity has the component of the specified type; otherwise, false.</returns>
    public bool Has(int entityId) => entityId < _sparse.Length && _sparse[entityId] != -1;

    /// <summary>
    /// Removes a component of the specified type from the given entity if it exists.
    /// </summary>
    /// <typeparam name="T">The type of the component to remove.</typeparam>
    /// <param name="entityId">The entity from which the component will be removed.</param>
    public void Remove(int entityId) {
        if (!Has(entityId)) {
            return;
        }

        int targetIndex = _sparse[entityId];
        _instances[targetIndex] = _instances[--Count];
        _dense[targetIndex] = _dense[Count];
        _sparse[_dense[targetIndex]] = targetIndex;
        _sparse[entityId] = -1;
    }

    /// <summary>
    /// Represents the total number of components currently stored in the component store.
    /// This value reflects the count of active components associated with entities and is updated whenever components are added or removed.
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// Retrieves a reference to the component at the specified index within the component store.
    /// </summary>
    /// <param name="index">The zero-based index of the component to retrieve.</param>
    /// <returns>A reference to the component at the specified index.</returns>
    public ref T GetByIndex(int index) => ref _instances[index];

    /// <summary>
    /// Retrieves the entity ID stored at the specified index in the internal dense array of the component store.
    /// </summary>
    /// <param name="index">The zero-based index from which to retrieve the entity ID.</param>
    /// <returns>The entity identifier located at the specified index.</returns>
    public int GetEntityIdAt(int index) => _dense[index];
}
