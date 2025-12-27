namespace Revolt.Core.Resource;

public enum ResourceType { Shader, Texture, Font, Sound }

public interface IResource {
    string Name { get; }
    void Unload();
}