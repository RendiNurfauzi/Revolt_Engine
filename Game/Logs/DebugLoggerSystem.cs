using Revolt.Core;
using Revolt.Engine;
using Revolt.Engine.ECS;
using System;

public class DebugLoggerSystem : GameModule
{
    public override string Name => "Debug_Logger";
    public override int Priority => 999; 

    private readonly CoreEngine _engine;
    private double _timer = 0;

    public DebugLoggerSystem(CoreEngine engine) => _engine = engine;

    public override void OnUpdate(double dt)
    {
        _timer += dt;
        if (_timer >= 1.0) // Log setiap 1 detik
        {
            var ecs = _engine.GetSystem<ECSSystem>();
            var transformPool = ecs.World.GetPool<ECSTransform>();

            // Cek apakah entitas 0 (Player) ada dan punya transform
            if (transformPool.Contains(0))
            {
                ref var tr = ref transformPool.Get(0);
                
                // Gunakan tr.Position (yang bertipe Vector3d)
                Console.WriteLine($"[Revolt Log] Player World Pos: X={tr.Position.X:F2}, Y={tr.Position.Y:F2}");
            }
            else
            {
                Console.WriteLine("[Revolt Log] Player (ID 0) not found in ECS World.");
            }
            
            _timer = 0;
        }
    }
}