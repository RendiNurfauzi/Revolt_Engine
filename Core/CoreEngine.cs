using System.Diagnostics;
using Revolt.Graphics;

namespace Revolt.Core;

public class CoreEngine
{
    private readonly List<IModule> _systems = new();
    private readonly Dictionary<Type, IModule> _systemRegistry = new();
    public bool IsRunning { get; private set; }
    private Stopwatch _timer = new();

    public void RegisterSystem<T>(T system) where T : IModule
    {
        _systems.Add(system);
        _systems.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        _systemRegistry[typeof(T)] = system;

        var interfaces = system.GetType().GetInterfaces();
        foreach (var i in interfaces) _systemRegistry[i] = system;

        system.OnAwake();
        Console.WriteLine($"[Core] Registered {system.GetType().BaseType?.Name}: {system.Name}");
    }

    public T GetSystem<T>()
    {
        if (_systemRegistry.TryGetValue(typeof(T), out var system))
            return (T)system;
        throw new Exception($"System {typeof(T).Name} tidak ditemukan!");
    }

    public void Run()
    {
        IsRunning = true;
        foreach (var system in _systems) system.OnStart();

        _timer.Start();
        double lastTime = 0;

        while (IsRunning)
        {
            double currentTime = _timer.Elapsed.TotalSeconds;
            double deltaTime = currentTime - lastTime;
            lastTime = currentTime;

            foreach (var system in _systems)
            {
                system.OnUpdate(deltaTime);

                // Auto-stop jika window ditutup
                if (system is IGraphicsSystem graphics && graphics.ShouldClose())
                    Stop();
            }
            Thread.Sleep(1);
        }

        foreach (var system in _systems) system.OnShutdown();
    }

    public void Stop() => IsRunning = false;
}