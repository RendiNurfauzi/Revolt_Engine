using System.Numerics;
using Revolt.Core;

namespace Revolt.Graphics;
public enum RenderMode
{
    World3D, // Perspective, Depth Test On
    UI2D     // Orthographic, Depth Test Off
}
public interface IGraphicsSystem
{
    bool IsHeadless { get; }
    string ApiName { get; }
    void ClearColor(float r, float g, float b, float a);
    void BeginFrame();
    void EndFrame();
    bool ShouldClose();
    void SetRenderMode(RenderMode mode);
    void SubmitBatch(float[] vertexData, uint[] indexData, int vertexCount, int indexCount);
    void SetShader(string shaderName);
    void UpdateViewMatrix(Vector3 position, Vector3 target); // Tambahkan janji baru
    void SetViewport(int x, int y, int width, int height);
}