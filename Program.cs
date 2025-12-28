using Revolt.Core;
using Revolt.Graphics;
using Revolt.Core.Input;
using Revolt.Engine;
using RevoltEngine.Graphics.OpenGL;
using Revolt.Graphics.OpenGL;
using Revolt.Game.Systems;
using Revolt.Engine.ECS;
using Revolt.Engine.Systems;
using System.Numerics;
using Revolt.Game.Transform;
using Revolt.Core.Math;
using Revolt.Game.Entities;
using Revolt.Editor;
using Revolt.Editor.Systems;

var engine = new CoreEngine();

// --- 1. RESOURCE & INFRASTRUCTURE ---
engine.RegisterSystem(new ResourceSystem());
var graphics = new OpenGLSystem(engine);
engine.RegisterSystem(graphics);

// Daftarkan Input System agar bisa dibaca oleh CameraSystem
var input = new OpenGLInputSystem(graphics.GetWindow());
engine.RegisterSystem(input);

// --- 2. ENGINE CORE ---
engine.RegisterSystem(new ECSSystem());
engine.RegisterSystem(new WorldSystem(engine));

// --- 3. GAME LOGIC & CAMERA ---
// CameraSystem ditaruh di sini agar posisi kamera diupdate sebelum render
engine.RegisterSystem(new CameraSystem(engine));
engine.RegisterSystem(new TransformSystem(engine));
engine.RegisterSystem(new MovementSystem(engine));

// --- 4. RENDERING & DEBUG ---
engine.RegisterSystem(new UIBatchRendererSystem(engine));
engine.RegisterSystem(new BatchRendererSystem(engine));
engine.RegisterSystem(new DebugLoggerSystem(engine));

// --- SETUP INITIAL DATA (3D STRESS TEST) ---
var ecs = engine.GetSystem<ECSSystem>();
var random = new Random();

// Spawn 50.000 Asteroid menggunakan Blueprint dari EntityFactory
for (int i = 0; i < 50000; i++)
{
    // Kita sebar di area yang luas
    var pos = new Vector3d(
        random.Next(-5000, 5000),
        random.Next(-5000, 5000),
        random.Next(-10000, 500) // Sebar jauh ke depan (Z negatif)
    );

    // Sekarang menggunakan factory, bukan AddComponent manual di sini
    float randomSize = (float)random.NextDouble() * 5.0f + 1.0f;
    EntityFactory.CreateAsteroid(ecs, pos, randomSize);
}

// Tambahkan Player menggunakan factory juga
EntityFactory.CreatePlayer(ecs, new Vector3d(0, 0, 0));

#if DEBUG
engine.RegisterSystem(new EditorSystem(engine));
#endif

// Jalankan Engine
engine.Run();