using Revolt.Core;

public abstract class GameModule : IModule
{
    public abstract string Name { get; }
    public abstract int Priority { get; }
    public virtual void OnAwake() { }
    public virtual void OnStart() { }
    public virtual void OnUpdate(double dt) { }
    public virtual void OnShutdown() { }
}