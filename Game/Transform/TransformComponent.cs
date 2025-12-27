using System.Numerics;

namespace Revolt.Engine.ECS;

public struct TransformComponent
{
    public Vector3 Position;
    public Vector3 Rotation; // Dalam derajat
    public Vector3 Scale;
    
    // Matrix hasil kalkulasi (dikirim ke Shader)
    public Matrix4x4 WorldMatrix;

    // Hierarchy
    public int ParentId; // -1 jika tidak punya parent

    public static TransformComponent Default => new() 
    { 
        Scale = Vector3.One, 
        ParentId = -1, 
        WorldMatrix = Matrix4x4.Identity 
    };
}