namespace Revolt.Core.Input;

public enum RevoltKey
{
    W, A, S, D, 
    Up, Down, Left, Right,
    Space, Escape, ShiftLeft
}

public interface IInputSystem
{
    bool IsKeyPressed(RevoltKey key);
    System.Numerics.Vector2 GetMousePosition();
}