# Sturdy Path

[![Tests](https://github.com/mpewsey/SturdyPath/actions/workflows/tests.yml/badge.svg)](https://github.com/mpewsey/SturdyPath/actions/workflows/tests.yml)
[![Docs](https://github.com/mpewsey/SturdyPath/actions/workflows/docs.yml/badge.svg?event=push)](https://github.com/mpewsey/SturdyPath/actions/workflows/docs.yml)
![Godot .NET 4.0](https://img.shields.io/badge/Godot%20.NET-4.2-blue)
![Version](https://img.shields.io/github/v/tag/mpewsey/SturdyPath?label=Version)

Sturdy Path is a simple Godot addon that provides a way to reference a scene or resource by path, in a manner that won't break when moving files around within the project. This is accomplished by keeping track of the target file's `uid://` path, which is invariant of the file's location within the project.

![image](https://github.com/user-attachments/assets/5efec515-aa99-4f41-a93e-d01a8d8e5a43)

## Installation

1. Copy the `addons/mpewsey.sturdypath` directory from this repository into the `addons` folder of your Godot project.
2. Build your Godot project solution, so that the newly incorporated C# addon scripts are compiled.
3. Enable the addon by going to `Project > Project Settings` and clicking the enable checkbox next to the plugin.

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
