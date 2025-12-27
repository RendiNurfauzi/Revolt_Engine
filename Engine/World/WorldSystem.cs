using Revolt.Core;
using Revolt.Core.Math;
using Revolt.Engine.ECS;
using System;

namespace Revolt.Engine.Systems;

public class WorldSystem : EngineModule
{
    public override string Name => "World_System";
    public override int Priority => 5; // Harus jalan paling awal sebelum Transform menghitung matriks

    private readonly CoreEngine _engine;
    
    // Titik pusat semesta saat ini (Double Precision)
    public Vector3d CurrentOrigin { get; private set; } = Vector3d.Zero;

    // Batas jarak sebelum kita melakukan shifting (misal 5000 unit/meter)
    private const double ShiftThreshold = 5000.0;

    public WorldSystem(CoreEngine engine) => _engine = engine;

    public override void OnUpdate(double dt)
    {
        var ecs = _engine.GetSystem<ECSSystem>();
        var transformPool = ecs.World.GetPool<ECSTransform>();

        // Asumsi: Entitas ID 0 adalah Player. 
        // Kedepannya kita bisa buat sistem 'Tag' atau 'Camera Target'
        if (transformPool.Contains(0))
        {
            ref var playerTr = ref transformPool.Get(0);
            
            // Hitung jarak player ke Origin saat ini
            double distanceToOrigin = (playerTr.Position - CurrentOrigin).Length();

            if (distanceToOrigin > ShiftThreshold)
            {
                // SHIFTING: Pindahkan Origin ke posisi Player sekarang
                CurrentOrigin = playerTr.Position;
                
                Console.WriteLine($"[World] Floating Origin Shifted to: {CurrentOrigin.X}, {CurrentOrigin.Y}");
            }
        }
    }
}