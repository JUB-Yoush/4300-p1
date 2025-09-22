using System;
using Godot;

public enum EffectName
{
    PURPLE,
    GREEN,
    SMALL,
    LARGE,
    TRIANGLE,
    CIRCLE,
}

public enum ScentEffect
{
    SIZE,
    SHAPE,
    COLOUR,
}

[GlobalClass]
public partial class ItemData : Resource
{
    [Export]
    public string name { get; set; } = "blah";

    [Export]
    public Texture2D UiSprite;

    [Export]
    public Color Color;

    [Export]
    public float Size;

    [Export]
    public Material Shape;
    [Export]
    public ScentEffect Effect;

    [Export]
    public EffectName EffectName;

    public ItemData() { }
}
