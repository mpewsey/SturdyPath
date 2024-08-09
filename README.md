# Sturdy Path

Sturdy Path is a Godot .NET addon that provides a way to reference a scene or resource by path, in a manner that won't break when moving files around within the project. This is accomplished by keeping track of the target file's `uid://` path, which is invariant of the file's location within the project.

![image](https://github.com/user-attachments/assets/5efec515-aa99-4f41-a93e-d01a8d8e5a43)

## Example Usage

```BattleEffect.cs
using MPewsey.SturdyPath;

public partial class BattleEffect : Resource
{
    // Create an exported SturdyPathRef property for the scene or resource you wish to reference by path.
    [Export] public SturdyPathRef VfxScenePath { get; set; }

    // Use the SturdyPathRef.Load method to load the resource for use per typical Godot usage patterns.
    public void SpawnVfx(Node parent)
    {
        var scene = VfxScenePath.Load<PackedScene>();
        var vfx = scene.Instantiate();
        parent.AddChild(vfx);
    }
}
```
