using System;
using Godot;

namespace Scent;

public partial class SmellItem : Node3D
{
    [Export]
    public ItemData? Data;

    public override void _Ready()
    {
        if (Data == null)
        {
            GD.PrintErr($"Item {this} missing ItemData Resource");
        }
        AddToGroup("items");
        GetNode<Sprite3D>("Sprite3D").Texture = Data!.Sprite;
    }
}
