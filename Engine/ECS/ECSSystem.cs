namespace Revolt.Engine;
using Revolt.Core;
using Revolt.Engine.ECS;

public class ECSSystem : EngineModule
{
    public override string Name => "ECS_Core";
    public override int Priority => 100;
    public ECSWorld World { get; } = new();
}