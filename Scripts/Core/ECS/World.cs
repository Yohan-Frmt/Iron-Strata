using System;
using System.Collections.Generic;
using System.Linq;

namespace IronStrata.Scripts.Core.ECS;

public class World
{
    private int _nextId;

    private readonly HashSet<int> _alive = [];
    private readonly Queue<int> _recycled = new();
    private readonly Dictionary<Type, Dictionary<int, IComponent>> _stores = new();
    public event Action<Entity> OnEntityCreated;
    public event Action<Entity> OnEntityDestroyed;


    public Entity CreateEntity()
    {
        var id = _recycled.Count > 0 ? _recycled.Dequeue() : _nextId++;
        _alive.Add(id);
        var entity = new Entity(id);
        OnEntityCreated?.Invoke(entity);
        return entity;
    }

    public void DestroyEntity(Entity entity)
    {
        if (!_alive.Contains(entity.Id)) return;
        foreach (var store in _stores.Values) store.Remove(entity.Id);
        _alive.Remove(entity.Id);
        _recycled.Enqueue(entity.Id);
        OnEntityDestroyed?.Invoke(entity);
    }

    public bool IsAlive(Entity entity) => _alive.Contains(entity.Id);

    public int EntityCount => _alive.Count;

    public void Add<T>(Entity entity, T component) where T : IComponent
    {
        var store = GetOrCreateStore<T>();
        store[entity.Id] = component;
    }

    public T Get<T>(Entity entity) where T : IComponent
    {
        return TryGet<T>(entity, out var component)
            ? component!
            : throw new InvalidOperationException($"Entity {entity.Id} does not have component {typeof(T).Name}");
    }

    public bool TryGet<T>(Entity entity, out T component) where T : IComponent
    {
        var type = typeof(T);
        if (_stores.TryGetValue(type, out var store) && store.TryGetValue(entity.Id, out var raw))
        {
            component = (T)raw;
            return true;
        }

        component = default;
        return false;
    }

    public bool Has<T>(Entity entity) where T : IComponent
    {
        var type = typeof(T);
        return _stores.TryGetValue(type, out var store) && store.ContainsKey(entity.Id);
    }

    public void Remove<T>(Entity entity) where T : IComponent
    {
        var type = typeof(T);
        if (_stores.TryGetValue(type, out var store))
            store.Remove(entity.Id);
    }

    public IEnumerable<Entity> Query<T>() where T : IComponent
    {
        var type = typeof(T);
        if (!_stores.TryGetValue(type, out var store)) yield break;
        foreach (var id in store.Keys.ToList().Where(id => _alive.Contains(id))) yield return new Entity(id);
    }

    public IEnumerable<Entity> Query<T1, T2>()
        where T1 : IComponent
        where T2 : IComponent
    {
        return Query<T1>().Where(Has<T2>);
    }

    public IEnumerable<Entity> Query<T1, T2, T3>()
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
    {
        return Query<T1, T2>().Where(Has<T3>);
    }

    private Dictionary<int, IComponent> GetOrCreateStore<T>() where T : IComponent
    {
        var type = typeof(T);
        if (_stores.TryGetValue(type, out var store)) return store;
        store = [];
        _stores[type] = store;
        return store;
    }
}