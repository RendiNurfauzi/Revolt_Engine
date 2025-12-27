namespace Revolt.Core;

public abstract class EngineModule : IModule
{
    public abstract string Name { get; }
    public abstract int Priority { get; }

    // Virtual body kosong agar anak cucu tidak wajib override (No Boilerplate!)
    public virtual void OnAwake() { }
    public virtual void OnStart() { }
    public virtual void OnUpdate(double dt) { }
    public virtual void OnShutdown() { }
}