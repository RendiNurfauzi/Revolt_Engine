using System.Numerics;
using Revolt.Core;
using Revolt.Engine.ECS;
using Revolt.Graphics;

namespace Revolt.Engine.Systems;

public class BatchRendererSystem : EngineModule
{
    public override string Name => "Batch_Renderer";
    public override int Priority => 500; // Render dijalankan paling akhir

    private readonly CoreEngine _engine;
    
    // Penampung data vertex yang dikirim ke GPU
    private float[] _vertexBatch = new float[20000]; // Kapasitas diperbesar
    private uint[] _indexBatch = new uint[10000];

    public BatchRendererSystem(CoreEngine engine) => _engine = engine;

    public override void OnUpdate(double dt)
    {
        var ecs = _engine.GetSystem<ECSSystem>();
        var graphics = _engine.GetSystem<IGraphicsSystem>();

        int vIndex = 0;
        int iIndex = 0;
        uint vOffset = 0;

        // Sekarang kita mengambil TransformComponent, bukan Position lagi
        var transformPool = ecs.World.GetPool<ECSTransform>();
        var entities = ecs.World.GetAllEntities();

        foreach (int entity in entities)
        {
            // Cek apakah entitas punya data transform
            if (!transformPool.Contains(entity)) continue;

            ref var transform = ref transformPool.Get(entity);
            
            // Ukuran dasar quad (bisa dipindah ke SpriteComponent nantinya)
            float size = 50f; 

            // --- TRANSFORMASI VERTEX MENGGUNAKAN WORLD MATRIX ---
            // Kita hitung 4 sudut kotak secara lokal (0 sampai size)
            // Lalu kalikan dengan WorldMatrix untuk mendapatkan posisi dunia yang benar (rotasi/skala/posisi)
            
            Vector3 pTL = Vector3.Transform(new Vector3(0, 0, 0), transform.WorldMatrix);       // Top Left
            Vector3 pTR = Vector3.Transform(new Vector3(size, 0, 0), transform.WorldMatrix);    // Top Right
            Vector3 pBR = Vector3.Transform(new Vector3(size, size, 0), transform.WorldMatrix); // Bottom Right
            Vector3 pBL = Vector3.Transform(new Vector3(0, size, 0), transform.WorldMatrix);    // Bottom Left

            // Warna (sementara hardcode merah, nanti bisa dari SpriteComponent)
            float r = 1f, g = 0f, b = 0f, a = 1f;

            // Masukkan ke Buffer: Position (3f) + Color (4f)
            
            // Vertex 0: Top Right
            AddVertex(ref vIndex, pTR, r, g, b, a);
            // Vertex 1: Bottom Right
            AddVertex(ref vIndex, pBR, r, g, b, a);
            // Vertex 2: Bottom Left
            AddVertex(ref vIndex, pBL, r, g, b, a);
            // Vertex 3: Top Left
            AddVertex(ref vIndex, pTL, r, g, b, a);

            // Indices (2 Segitiga membentuk 1 Kotak)
            _indexBatch[iIndex++] = vOffset + 0;
            _indexBatch[iIndex++] = vOffset + 1;
            _indexBatch[iIndex++] = vOffset + 2;
            
            _indexBatch[iIndex++] = vOffset + 2;
            _indexBatch[iIndex++] = vOffset + 3;
            _indexBatch[iIndex++] = vOffset + 0;

            vOffset += 4;
        }

        // Kirim semua data yang terkumpul ke GPU dalam satu kali panggil
        if (iIndex > 0)
        {
            graphics.SubmitBatch(_vertexBatch, _indexBatch, vIndex, iIndex);
        }

        graphics.EndFrame();
    }

    // Helper agar kode lebih bersih saat mengisi array vertex
    private void AddVertex(ref int index, Vector3 pos, float r, float g, float b, float a)
    {
        _vertexBatch[index++] = pos.X;
        _vertexBatch[index++] = pos.Y;
        _vertexBatch[index++] = pos.Z;
        _vertexBatch[index++] = r;
        _vertexBatch[index++] = g;
        _vertexBatch[index++] = b;
        _vertexBatch[index++] = a;
    }
}