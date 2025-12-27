using System.Numerics;
using Revolt.Core;
using Revolt.Core.Input;
using Revolt.Engine.ECS;
using Revolt.Engine;

namespace Revolt.Game.Systems;

public class MovementSystem : GameModule
{
    public override string Name => "Movement_Logic";
    public override int Priority => 200; // Berjalan setelah Input, sebelum Transform & Render
    private readonly CoreEngine _engine;

    public MovementSystem(CoreEngine engine) => _engine = engine;

    public override void OnUpdate(double dt)
    {
        var input = _engine.GetSystem<IInputSystem>();
        var ecs = _engine.GetSystem<ECSSystem>();

        // 1. Ambil Pool TransformComponent (bukan Position)
        var transformPool = ecs.World.GetPool<TransformComponent>();
        
        // 2. Ambil semua entitas yang aktif
        var entities = ecs.World.GetAllEntities();

        float speed = 250f; // Kecepatan pixel per detik

        foreach (var entity in entities)
        {
            // 3. Sabuk Pengaman: Cek apakah entitas ini punya Transform
            if (!transformPool.Contains(entity)) continue;

            // 4. Ambil referensi datanya
            ref var transform = ref transformPool.Get(entity);

            // 5. Logika pergerakan WASD
            if (input.IsKeyPressed(RevoltKey.W)) transform.Position.Y -= speed * (float)dt;
            if (input.IsKeyPressed(RevoltKey.S)) transform.Position.Y += speed * (float)dt;
            if (input.IsKeyPressed(RevoltKey.A)) transform.Position.X -= speed * (float)dt;
            if (input.IsKeyPressed(RevoltKey.D)) transform.Position.X += speed * (float)dt;

            // Debug: Coba rotasi otomatis untuk memastikan TransformSystem bekerja
            // transform.Rotation.Z += 45f * (float)dt; 
        }
    }
}