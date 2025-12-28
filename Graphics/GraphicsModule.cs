namespace Revolt.Graphics;

using System.Numerics;
using Revolt.Core;

public abstract class GraphicsModule : EngineModule, IGraphicsSystem
{
    // Metadata default
    public virtual bool IsHeadless => false;
    public virtual string ApiName => "Generic_API";

    // Method virtual kosong - Implementasi boleh override hanya yang butuh
    public virtual void ClearColor(float r, float g, float b, float a) { }
    public virtual void BeginFrame() { }
    public virtual void EndFrame() { }
    public virtual bool ShouldClose() => false;
    public virtual void SetRenderMode(RenderMode mode) { }
    public virtual void SetShader(string shaderName) { }
    public virtual void SubmitBatch(float[] vertexData, uint[] indexData, int vertexCount, int indexCount) { }
    public virtual void UpdateViewMatrix(Vector3 position, Vector3 target) { }
    public virtual void SetViewport(int x, int y, int width, int height) {}
}