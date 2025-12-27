using System.Numerics;
using Revolt.Core.Math;

namespace Revolt.Engine.ECS;

public struct ECSTransform
{
    public Vector3d Position;
    public Vector3 Rotation; // Dalam derajat
    public Vector3 Scale;

    // Matrix hasil kalkulasi (dikirim ke Shader)
    public Matrix4x4 WorldMatrix;

    // Hierarchy
    public int ParentId; // -1 jika tidak punya parent

    public static ECSTransform Default => new()
    {
        Scale = Vector3.One,
        ParentId = -1,
        WorldMatrix = Matrix4x4.Identity
    };
}