using System;

namespace Revolt.Engine.ECS; // Pastikan ini sama dengan di ECSWorld

public class ComponentPool<T> where T : struct
{
    private T[] _instances = new T[1024]; 
    private int[] _sparse = new int[4096]; 
    private int[] _dense = new int[1024];  
    private int _size = 0;

    public ComponentPool()
    {
        Array.Fill(_sparse, -1);
    }

    public void Add(int entityId, T component)
    {
        if (_size >= _instances.Length) 
        {
            Array.Resize(ref _instances, _instances.Length * 2);
            Array.Resize(ref _dense, _dense.Length * 2);
        }
        
        // Resize sparse jika entityId melebihi kapasitas
        if (entityId >= _sparse.Length)
        {
            int oldSize = _sparse.Length;
            Array.Resize(ref _sparse, entityId + 1024);
            for (int i = oldSize; i < _sparse.Length; i++) _sparse[i] = -1;
        }

        _sparse[entityId] = _size;
        _dense[_size] = entityId;
        _instances[_size] = component;
        _size++;
    }

    // Method ini yang dicari oleh BatchRendererSystem
    public bool Contains(int entityId) 
    {
        if (entityId < 0 || entityId >= _sparse.Length) return false;
        int index = _sparse[entityId];
        return index >= 0 && index < _size && _dense[index] == entityId;
    }

    public ref T Get(int entityId) => ref _instances[_sparse[entityId]];
    
    public Span<T> GetAll() => _instances.AsSpan(0, _size);
}