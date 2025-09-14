using System;
using Godot;

public enum ScentColor
{
    RED,
    GREEN,
    BLUE,
    GREY,
}

public enum ScentSize
{
    SMALL,
    MEDIUM,
    LARGE,
    MIXED,
}

public enum ScentShape
{
    TRIANGLE,
    SQUARE,
    CIRCLE,
    MIXED,
}

[GlobalClass]
public partial class ItemData : Resource
{
    [Export]
    public string name { get; set; } = "blah";

    [Export]
    public Texture2D Sprite;

    [Export]
    public Color Color;

    [Export]
    public float Size;

    [Export]
    public Material Shape;

    public ItemData() { }
}
