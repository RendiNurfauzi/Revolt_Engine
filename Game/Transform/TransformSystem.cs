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

    private readonly CoreEngine _engine;
    public TransformSystem(CoreEngine engine) => _engine = engine;

    public override void OnUpdate(double dt)
    {
        var ecs = _engine.GetSystem<ECSSystem>();
        var world = _engine.GetSystem<WorldSystem>(); // Ambil WorldSystem untuk akses Origin
        var entities = ecs.World.GetAllEntities();
        var transformPool = ecs.World.GetPool<ECSTransform>();

        foreach (var entity in entities)
        {
            if (!transformPool.Contains(entity)) continue;

            ref var transform = ref transformPool.Get(entity);
            
            // --- FIX ERROR CS1503: HITUNG POSISI RELATIF ---
            // Kita kurangi posisi absolut dengan Origin saat ini, lalu konversi ke float (Vector3)
            Vector3 relativePos = (transform.Position - world.CurrentOrigin).ToVector3();

            // 1. Hitung Local Matrix menggunakan relativePos (Aman dari Jitter)
            Matrix4x4 localMatrix = Matrix4x4.CreateScale(transform.Scale) *
                                    Matrix4x4.CreateRotationZ(MathHelper.ToRadians(transform.Rotation.Z)) *
                                    Matrix4x4.CreateTranslation(relativePos);

            // 2. Hierarchy Logic (Tetap sama)
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
    }
}

// Helper untuk konversi derajat ke radian
public static class MathHelper {
    public static float ToRadians(float degrees) => degrees * (MathF.PI / 180f);
}