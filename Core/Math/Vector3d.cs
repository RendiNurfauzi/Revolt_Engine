using System.Numerics;

namespace Revolt.Core.Math;

public struct Vector3d
{
    public double X, Y, Z;

    public Vector3d(double x, double y, double z) { X = x; Y = y; Z = z; }

    // Konversi ke Vector3 float (Hanya untuk Renderer/Dapur)
    public Vector3 ToVector3() => new((float)X, (float)Y, (float)Z);

    // Operator dasar agar user enak pakainya
    public static Vector3d operator +(Vector3d a, Vector3d b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public static Vector3d operator -(Vector3d a, Vector3d b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    public static Vector3d Zero => new(0, 0, 0);
    
    public double Length() => System.Math.Sqrt(X * X + Y * Y + Z * Z);
}