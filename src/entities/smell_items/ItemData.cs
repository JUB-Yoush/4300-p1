using System;
using Godot;

public enum ScentColor
{
    RED,
    GREEN,
    BLUE,
}

public enum ScentSize
{
    SMALL,
    MEDIUM,
    LARGE,
}

public enum ScentShape
{
    TRIANGLE,
    SQUARE,
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
    public Texture2D Sprite, UiSprite;

    [Export]
    public Color Color;

    [Export]
    public float Size;

    [Export]
    public Material Shape;

    [Export]
    public ScentEffect Effect;

    public ItemData() { }
}
