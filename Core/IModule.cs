namespace Revolt.Core;

public interface IModule
{
    string Name { get; }
    int Priority { get; }
    void OnAwake();
    void OnStart();
    void OnUpdate(double dt);
    void OnShutdown();
}