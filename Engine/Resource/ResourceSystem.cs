using Revolt.Core;
using Revolt.Core.Resource;

namespace Revolt.Engine.Systems;
public class ResourceSystem : EngineModule
{
    public override string Name => "Resource_Manager";
    public override int Priority => 50; // Berjalan sangat awal

    private readonly Dictionary<string, IResource> _resourceCache = new();

    public override void OnAwake()
    {
        Console.WriteLine("[Resource] System Ready.");
    }

    // Generic Load: Bisa buat Shader, Texture, dll
    public T GetOrCreate<T>(string name, Func<T> loader) where T : IResource
    {
        if (_resourceCache.TryGetValue(name, out var res))
        {
            return (T)res;
        }

        T newResource = loader();
        _resourceCache[name] = newResource;
        return newResource;
    }

    public override void OnShutdown()
    {
        foreach (var res in _resourceCache.Values) res.Unload();
        _resourceCache.Clear();
    }
}