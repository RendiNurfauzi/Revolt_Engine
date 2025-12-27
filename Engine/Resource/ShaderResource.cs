
using Revolt.Core.Resource;

namespace Revolt.Engine.Resources;
public class ShaderResource : IResource
{
    public required string Name { get; init; }
    public uint ProgramHandle { get; init; }

    public void Unload() 
    {
        Console.WriteLine($"[Resource] Shader {Name} unloaded from GPU.");
    }
}