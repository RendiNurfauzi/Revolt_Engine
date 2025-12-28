namespace Revolt.Core.Input;

public enum RevoltKey
{
    W, A, S, D, 
    Up, Down, Left, Right,
    Space, Escape, ShiftLeft
}

public interface IInputSystem
{
    // Cek apakah tombol sedang ditahan (untuk jalan/terbang)
    bool IsKeyDown(RevoltKey key);
    
    // Cek apakah tombol baru saja ditekan (untuk tembak/menu)
    bool IsKeyPressed(RevoltKey key);
    
    System.Numerics.Vector2 GetMousePosition();
}