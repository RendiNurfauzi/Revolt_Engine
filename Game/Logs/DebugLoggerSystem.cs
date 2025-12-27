using Revolt.Core;
using Revolt.Engine;
using Revolt.Engine.ECS;

public class DebugLoggerSystem : GameModule
{
    public override string Name => "Debug_Logger";
    public override int Priority => 999; // Dijalankan paling akhir

    private readonly CoreEngine _engine; // Ganti dari ECSSystem ke CoreEngine
    private double _timer = 0;

    public DebugLoggerSystem(CoreEngine engine) => _engine = engine;

    public override void OnUpdate(double dt)
    {
        _timer += dt;
        if (_timer >= 1.0)
        {
            // Ambil ECSSystem via Service Locator (CoreEngine)
            var ecs = _engine.GetSystem<ECSSystem>();
            var pos = ecs.World.GetPool<Position>().Get(0);

            Console.WriteLine($"[Revolt Log] Player Position: X={pos.X:F2}, Y={pos.Y:F2}");
            _timer = 0;
        }
    }

}