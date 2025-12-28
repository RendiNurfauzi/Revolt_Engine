namespace Revolt.Editor;

using Revolt.Core;

public abstract class EditorModule : IModule
{
    public abstract string Name { get; }
    
    // Priority tinggi (misal 900+) agar Editor selalu diproses 
    // setelah logic game dan sistem engine selesai.
    public abstract int Priority { get; }

    public virtual void OnAwake() { }
    public virtual void OnStart() { }
    public virtual void OnUpdate(double dt) { }
    public virtual void OnShutdown() { }
}