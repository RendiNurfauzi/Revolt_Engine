using System.Numerics;
using Revolt.Core;
using Revolt.Engine;
using Revolt.Engine.ECS;
using Revolt.Engine.Systems; // WAJIB: Untuk ambil WorldSystem (Origin)

namespace Revolt.Game.Transform;

public class TransformSystem : GameModule
{
    public override string Name => "Transform_System";
    public override int Priority => 100;

    public bool UseMultiThreading = true;
    private readonly CoreEngine _engine;
    public TransformSystem(CoreEngine engine) => _engine = engine;


    public override void OnUpdate(double dt)
    {
        var ecs = _engine.GetSystem<ECSSystem>();
        var world = _engine.GetSystem<WorldSystem>();
        var entities = ecs.World.GetAllEntities();
        var transformPool = ecs.World.GetPool<ECSTransform>();

        // Logic internal untuk kalkulasi satu entitas
        void UpdateTransform(int i)
        {
            int entity = entities[i];
            if (!transformPool.Contains(entity)) return;

            ref var transform = ref transformPool.Get(entity);

            // Hitung Relative Position (Anti-Jitter)
            var relativePos = (transform.Position - world.CurrentOrigin).ToVector3();

            var localMatrix = System.Numerics.Matrix4x4.CreateScale(transform.Scale) *
                              System.Numerics.Matrix4x4.CreateRotationZ(MathHelper.ToRadians(transform.Rotation.Z)) *
                              System.Numerics.Matrix4x4.CreateTranslation(relativePos);

            // Hierarchy sederhana
            if (transform.ParentId != -1 && transformPool.Contains(transform.ParentId))
            {
                var parentTransform = transformPool.Get(transform.ParentId);
                transform.WorldMatrix = localMatrix * parentTransform.WorldMatrix;
            }
            else
            {
                transform.WorldMatrix = localMatrix;
            }
        }

        if (UseMultiThreading)
        {
            // MODE AAA: Bagi tugas ke semua CORE CPU
            Parallel.For(0, entities.Count, i =>
            {
                UpdateTransform(i);
            });
        }
        else
        {
            // MODE STANDAR: Satu per satu (Single Thread)
            for (int i = 0; i < entities.Count; i++)
            {
                UpdateTransform(i);
            }
        }
    }
}

// Helper untuk konversi derajat ke radian
public static class MathHelper
{
    public static float ToRadians(float degrees) => degrees * (MathF.PI / 180f);
}