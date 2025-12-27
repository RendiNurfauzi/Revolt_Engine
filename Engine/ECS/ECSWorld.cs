using System;
using System.Collections.Generic;

namespace Revolt.Engine.ECS;

public class ECSWorld
{
    private int _nextEntityId = 0;
    private readonly Dictionary<Type, object> _pools = new();
    // Melacak semua entitas yang pernah dibuat
    private readonly List<int> _entities = new();

    public int CreateEntity() 
    {
        int id = _nextEntityId++;
        _entities.Add(id);
        return id;
    }

    // Digunakan oleh Renderer/Movement System untuk iterasi
    public List<int> GetAllEntities() => _entities;

    public void AddComponent<T>(int entityId, T data) where T : struct
    {
        var type = typeof(T);
        if (!_pools.ContainsKey(type)) _pools[type] = new ComponentPool<T>();
        
        ((ComponentPool<T>)_pools[type]).Add(entityId, data);
    }

    public ComponentPool<T> GetPool<T>() where T : struct
    {
        var type = typeof(T);
        if (!_pools.TryGetValue(type, out var pool))
        {
            // Auto-create pool jika belum ada agar tidak crash saat GetSystem
            pool = new ComponentPool<T>();
            _pools[type] = pool;
        }
        return (ComponentPool<T>)pool;
    }
}