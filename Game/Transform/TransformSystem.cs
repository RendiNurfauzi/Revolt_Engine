using System.Numerics;
using Revolt.Core;
using Revolt.Engine;
using Revolt.Engine.ECS;

namespace Revolt.Game.Systems;

public class TransformSystem : GameModule
{
    public override string Name => "Transform_System";
    public override int Priority => 100; // Berjalan sebelum Renderer

    private readonly CoreEngine _engine;

    public TransformSystem(CoreEngine engine) => _engine = engine;

    public override void OnUpdate(double dt)
    {
        var ecs = _engine.GetSystem<ECSSystem>();
        var entities = ecs.World.GetAllEntities();
        var transformPool = ecs.World.GetPool<TransformComponent>();

        foreach (var entity in entities)
        {
            if (!transformPool.Contains(entity)) continue;

            ref var transform = ref transformPool.Get(entity);
            
            // 1. Hitung Local Matrix (Scale -> Rotate -> Translate)
            Matrix4x4 localMatrix = Matrix4x4.CreateScale(transform.Scale) *
                                    Matrix4x4.CreateRotationZ(MathHelper.ToRadians(transform.Rotation.Z)) *
                                    Matrix4x4.CreateTranslation(transform.Position);

            // 2. Jika punya Parent, kalikan dengan WorldMatrix Parent
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