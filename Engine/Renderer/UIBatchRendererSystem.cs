using System.Numerics;
using Revolt.Core;
using Revolt.Graphics;

namespace Revolt.Editor.Systems;

public class UIBatchRendererSystem : EditorModule
{
    public override string Name => "UI_Batch_Renderer";
    public override int Priority => 1100;

    private readonly CoreEngine _engine;

    // Buffer Data
    private const int MaxQuads = 2000;
    private const int FloatsPerVertex = 7; // Pos(3) + Color(4)
    private readonly float[] _vertexBatch = new float[MaxQuads * 4 * FloatsPerVertex];
    private readonly uint[] _indexBatch = new uint[MaxQuads * 6];

    private int _vIndex = 0;
    private int _iIndex = 0;
    private uint _vOffset = 0;
    private int _quadCount = 0;

    public UIBatchRendererSystem(CoreEngine engine) => _engine = engine;

    public override void OnUpdate(double dt)
    {
        // OnUpdate di sini bisa digunakan untuk Reset, 
        // tapi Flush dilakukan di akhir frame oleh CoreEngine secara berurutan
    }

    // Fungsi ini dipanggil oleh EditorSystem (Immediate Mode Style)
    public void DrawRect(float x, float y, float w, float h, Vector4 col)
    {
        if (_quadCount >= MaxQuads) Flush();

        // Counter-Clockwise Vertices
        AddVertex(new Vector3(x, y + h, 0), col);     // Bottom Left
        AddVertex(new Vector3(x + w, y + h, 0), col); // Bottom Right
        AddVertex(new Vector3(x + w, y, 0), col);     // Top Right
        AddVertex(new Vector3(x, y, 0), col);         // Top Left

        _indexBatch[_iIndex++] = _vOffset + 0;
        _indexBatch[_iIndex++] = _vOffset + 1;
        _indexBatch[_iIndex++] = _vOffset + 2;
        _indexBatch[_iIndex++] = _vOffset + 2;
        _indexBatch[_iIndex++] = _vOffset + 3;
        _indexBatch[_iIndex++] = _vOffset + 0;

        _vOffset += 4;
        _quadCount++;
    }

    public void StartFrame()
    {
        _vIndex = 0;
        _iIndex = 0;
        _vOffset = 0;
        _quadCount = 0;
    }

    public void EndFrame()
    {
        if (_iIndex > 0) Flush();
    }

    private void Flush()
    {
        var graphics = _engine.GetSystem<IGraphicsSystem>();
        graphics.SetRenderMode(RenderMode.UI2D);
        graphics.SubmitBatch(_vertexBatch, _indexBatch, _vIndex, _iIndex);
        graphics.SetRenderMode(RenderMode.World3D); // Balikkan ke 3D
        
        // Reset batch setelah flush jika dalam satu frame UI-nya sangat banyak
        _vIndex = 0; _iIndex = 0; _vOffset = 0; _quadCount = 0;
    }

    private void AddVertex(Vector3 pos, Vector4 col)
    {
        _vertexBatch[_vIndex++] = pos.X;
        _vertexBatch[_vIndex++] = pos.Y;
        _vertexBatch[_vIndex++] = pos.Z;
        _vertexBatch[_vIndex++] = col.X;
        _vertexBatch[_vIndex++] = col.Y;
        _vertexBatch[_vIndex++] = col.Z;
        _vertexBatch[_vIndex++] = col.W;
    }
}