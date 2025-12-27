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

var engine = new CoreEngine();

// --- 1. DAFTARKAN RESOURCE SYSTEM DULU ---
// Ini harus nomor satu karena sistem lain (Graphics) akan mencarinya saat Awake
engine.RegisterSystem(new ResourceSystem());

// --- 2. INFRASTRUCTURE ---
// Sekarang OpenGLSystem bisa menemukan ResourceSystem tanpa error
var graphics = new OpenGLSystem(engine);
engine.RegisterSystem(graphics); 

engine.RegisterSystem(new OpenGLInputSystem(graphics.GetWindow()));

// --- 3. ENGINE CORE ---
engine.RegisterSystem(new ECSSystem());
engine.RegisterSystem(new WorldSystem(engine)); // <--- Tambahkan ini

// --- 4. GAME LOGIC ---
// TransformSystem HARUS ADA agar WorldMatrix dihitung sebelum digambar
engine.RegisterSystem(new TransformSystem(engine));
engine.RegisterSystem(new MovementSystem(engine));

// --- 5. RENDERING ---
engine.RegisterSystem(new BatchRendererSystem(engine));

engine.RegisterSystem(new DebugLoggerSystem(engine));

// --- SETUP INITIAL DATA ---
var ecs = engine.GetSystem<ECSSystem>();
int p = ecs.World.CreateEntity();

// Pastikan menggunakan TransformComponent, bukan Position lagi
ecs.World.AddComponent(p, new ECSTransform { 
    Position = new Vector3d(4950.0, 0, 0),
    Scale = new Vector3(1, 1, 1),
    Rotation = Vector3.Zero,
    ParentId = -1 
});

engine.Run();