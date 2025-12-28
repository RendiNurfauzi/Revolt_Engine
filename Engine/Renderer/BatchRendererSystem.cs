using System.Numerics;
using Revolt.Core;
using Revolt.Engine.ECS;
using Revolt.Graphics;

namespace Revolt.Engine.Systems;

public class BatchRendererSystem : EngineModule
{
    public override string Name => "Batch_Renderer";
    public override int Priority => 500;

    private readonly CoreEngine _engine;

    // --- KONFIGURASI BATCH ---
    private const int MaxQuads = 2500; // Batas per satu tarikan gambar (Draw Call)
    private const int VerticesPerQuad = 4;
    private const int IndicesPerQuad = 6;
    private const int FloatsPerVertex = 7; // Pos(3) + Color(4)

    private readonly float[] _vertexBatch = new float[MaxQuads * VerticesPerQuad * FloatsPerVertex];
    private readonly uint[] _indexBatch = new uint[MaxQuads * IndicesPerQuad];

    private int _vIndex = 0;
    private int _iIndex = 0;
    private uint _vOffset = 0;
    private int _quadCount = 0;

    public BatchRendererSystem(CoreEngine engine) => _engine = engine;

    public override void OnUpdate(double dt)
    {
        var ecs = _engine.GetSystem<ECSSystem>();
        var graphics = _engine.GetSystem<IGraphicsSystem>();

        var transformPool = ecs.World.GetPool<ECSTransform>();
        var entities = ecs.World.GetAllEntities();

        // Reset index untuk frame baru
        StartBatch();

        foreach (int entity in entities)
        {
            if (!transformPool.Contains(entity)) continue;

            // --- AUTO-FLUSH LOGIC ---
            // Jika penampung penuh, kirim ke GPU sekarang juga!
            if (_quadCount >= MaxQuads)
            {
                Flush(graphics);
            }

            ref var transform = ref transformPool.Get(entity);
            // Di dalam loop foreach BatchRendererSystem
            float s = 1; // half-size

            // Kita buat 4 titik koordinat lokal (Z sekarang ikut serta)
            Vector3 pTL = Vector3.Transform(new Vector3(-s, s, 0), transform.WorldMatrix);
            Vector3 pTR = Vector3.Transform(new Vector3(s, s, 0), transform.WorldMatrix);
            Vector3 pBR = Vector3.Transform(new Vector3(s, -s, 0), transform.WorldMatrix);
            Vector3 pBL = Vector3.Transform(new Vector3(-s, -s, 0), transform.WorldMatrix);

            float r = 1f, g = 0f, b = 0f, a = 1f;

            // Tambah Vertices
            AddVertex(pTR, r, g, b, a);
            AddVertex(pBR, r, g, b, a);
            AddVertex(pBL, r, g, b, a);
            AddVertex(pTL, r, g, b, a);

            // Tambah Indices
            _indexBatch[_iIndex++] = _vOffset + 0;
            _indexBatch[_iIndex++] = _vOffset + 1;
            _indexBatch[_iIndex++] = _vOffset + 2;
            _indexBatch[_iIndex++] = _vOffset + 2;
            _indexBatch[_iIndex++] = _vOffset + 3;
            _indexBatch[_iIndex++] = _vOffset + 0;

            _vOffset += 4;
            _quadCount++;
        }

        // Gambar sisa entitas yang belum ter-flush
        if (_iIndex > 0)
        {
            Flush(graphics);
        }

        graphics.EndFrame();
    }

    private void StartBatch()
    {
        _vIndex = 0;
        _iIndex = 0;
        _vOffset = 0;
        _quadCount = 0;
    }

    private void Flush(IGraphicsSystem graphics)
    {
        // Kirim batch saat ini ke GPU
        graphics.SubmitBatch(_vertexBatch, _indexBatch, _vIndex, _iIndex);

        // Reset state untuk batch berikutnya dalam frame yang sama
        StartBatch();
    }

    private void AddVertex(Vector3 pos, float r, float g, float b, float a)
    {
        _vertexBatch[_vIndex++] = pos.X;
        _vertexBatch[_vIndex++] = pos.Y;
        _vertexBatch[_vIndex++] = pos.Z;
        _vertexBatch[_vIndex++] = r;
        _vertexBatch[_vIndex++] = g;
        _vertexBatch[_vIndex++] = b;
        _vertexBatch[_vIndex++] = a;
    }
}