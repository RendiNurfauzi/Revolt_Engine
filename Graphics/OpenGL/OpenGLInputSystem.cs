using Silk.NET.Input;
using Revolt.Core.Input;
using Silk.NET.Windowing;
using Revolt.Core;

namespace Revolt.Graphics.OpenGL;

public class OpenGLInputSystem : InputModule
{
    public override string Name => "Input_System";
    public override int Priority => 10;

    private IInputContext _inputContext;
    private readonly Dictionary<RevoltKey, bool> _keyStates = new();

    public OpenGLInputSystem(IWindow window)
    {
        // Inisialisasi input context dari window Silk.NET
        _inputContext = window.CreateInput();

        foreach (var keyboard in _inputContext.Keyboards)
        {
            // Event listener untuk tombol ditekan dan dilepas
            keyboard.KeyDown += (k, key, i) => OnKeyChange(key, true);
            keyboard.KeyUp += (k, key, i) => OnKeyChange(key, false);
        }
    }
    public override bool IsMouseButtonDown(RevoltMouseButton button)
    {
        if (_inputContext.Mice.Count > 0)
        {
            // Konversi dari Revolt Enum ke Silk.NET Enum
            var silkButton = button switch
            {
                RevoltMouseButton.Left => Silk.NET.Input.MouseButton.Left,
                RevoltMouseButton.Right => Silk.NET.Input.MouseButton.Right,
                RevoltMouseButton.Middle => Silk.NET.Input.MouseButton.Middle,
                _ => Silk.NET.Input.MouseButton.Unknown
            };

            return _inputContext.Mice[0].IsButtonPressed(silkButton);
        }
        return false;
    }
    private void OnKeyChange(Key key, bool isDown)
    {
        var revoltKey = key switch
        {
            Key.W => RevoltKey.W,
            Key.A => RevoltKey.A,
            Key.S => RevoltKey.S,
            Key.D => RevoltKey.D,
            Key.Up => RevoltKey.Up,
            Key.Down => RevoltKey.Down,
            Key.Left => RevoltKey.Left,
            Key.Right => RevoltKey.Right,
            Key.Space => RevoltKey.Space,
            Key.ShiftLeft => RevoltKey.ShiftLeft,
            Key.Escape => RevoltKey.Escape,
            _ => (RevoltKey?)null
        };

        if (revoltKey.HasValue) _keyStates[revoltKey.Value] = isDown;
    }

    public override bool IsKeyPressed(RevoltKey key) => _keyStates.GetValueOrDefault(key, false);
    public override bool IsKeyDown(RevoltKey key) => _keyStates.GetValueOrDefault(key, false);
    public bool IsMouseButtonDown(RevoltKey key) => _keyStates.GetValueOrDefault(key, false);

    public override System.Numerics.Vector2 GetMousePosition()
    {
        if (_inputContext.Mice.Count > 0)
        {
            var pos = _inputContext.Mice[0].Position;
            return new System.Numerics.Vector2(pos.X, pos.Y);
        }
        return System.Numerics.Vector2.Zero;
    }

    // Hanya override Shutdown untuk cleanup memori
    public override void OnShutdown()
    {
        _inputContext?.Dispose();
        Console.WriteLine("[Input] Context Disposed.");
    }
}