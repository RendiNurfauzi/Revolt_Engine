using Revolt.Core;

namespace Revolt.Graphics;

public interface IGraphicsSystem
{
    bool IsHeadless { get; }
    string ApiName { get; }
    void ClearColor(float r, float g, float b, float a);
    void BeginFrame();
    void EndFrame();
    bool ShouldClose();
    void SubmitBatch(float[] vertexData, uint[] indexData, int vertexCount, int indexCount);
    void SetShader(string shaderName);
}