using Revolt.Engine.ECS;
using Revolt.Core.Math;
using System.Numerics;
using Revolt.Engine;

namespace Revolt.Game.Entities;

public static class EntityFactory
{
    // Gunakan satu instance Random untuk semua agar angka benar-benar acak
    private static readonly Random _rng = new Random();

    public static int CreatePlayer(ECSSystem ecs, Vector3d position)
    {
        int entity = ecs.World.CreateEntity();
        
        ecs.World.AddComponent(entity, new ECSTransform 
        { 
            Position = position,
            Scale = Vector3.One * 10f, // Buat agak besar agar terlihat
            Rotation = Vector3.Zero,
            ParentId = -1
        });
        
        return entity;
    }

    public static int CreateAsteroid(ECSSystem ecs, Vector3d position, float size)
    {
        int entity = ecs.World.CreateEntity();
        
        ecs.World.AddComponent(entity, new ECSTransform 
        { 
            Position = position,
            Scale = new Vector3(size),
            // Gunakan _rng yang static agar rotasi bervariasi
            Rotation = new Vector3(0, 0, (float)_rng.NextDouble() * 6.28f), // Pakai Radian lebih aman
            ParentId = -1 // WAJIB: Agar TransformSystem tahu ini root object
        });
        
        return entity;
    }
}