namespace Revolt.Core.Input;

using Revolt.Core;

public abstract class InputModule : EngineModule, IInputSystem
{
    public virtual bool IsKeyDown(RevoltKey key) => false;
    public virtual bool IsKeyPressed(RevoltKey key) => false;
    public virtual bool IsMouseButtonDown(RevoltMouseButton button) => false;
    public virtual System.Numerics.Vector2 GetMousePosition() => System.Numerics.Vector2.Zero;
}