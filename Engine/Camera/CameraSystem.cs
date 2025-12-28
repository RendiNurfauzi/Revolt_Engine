using System.Numerics;
using Revolt.Core;
using Revolt.Core.Input;
using RevoltEngine.Graphics.OpenGL;

namespace Revolt.Engine.Systems;

public class CameraSystem : EngineModule
{
    public override string Name => "Camera_System";
    public override int Priority => 10; // Jalan sebelum Renderer

    private Vector3 _cameraPos = new Vector3(0, 0, 1000f);
    private float _speed = 500f; // Kecepatan terbang

    private readonly CoreEngine _engine;
    public CameraSystem(CoreEngine engine) => _engine = engine;

    public Vector3 GetPosition() => _cameraPos;

    public override void OnUpdate(double dt)
    {
        var input = _engine.GetSystem<IInputSystem>();
        float delta = (float)dt;

        // Kontrol Terbang 3D
        if (input.IsKeyDown(RevoltKey.W)) _cameraPos.Z -= _speed * delta; // Maju
        if (input.IsKeyDown(RevoltKey.S)) _cameraPos.Z += _speed * delta; // Mundur
        if (input.IsKeyDown(RevoltKey.A)) _cameraPos.X -= _speed * delta; // Kiri
        if (input.IsKeyDown(RevoltKey.D)) _cameraPos.X += _speed * delta; // Kanan
        if (input.IsKeyDown(RevoltKey.Space)) _cameraPos.Y += _speed * delta; // Naik
        if (input.IsKeyDown(RevoltKey.ShiftLeft)) _cameraPos.Y -= _speed * delta; // Turun

        // Kirim posisi terbaru ke OpenGLSystem agar matriks View diupdate
        var graphics = _engine.GetSystem<OpenGLSystem>();
        graphics.UpdateViewMatrix(_cameraPos, _cameraPos + new Vector3(0, 0, -1));
    }
}