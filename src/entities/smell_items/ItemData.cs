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

[GlobalClass]
public partial class ItemData : Resource
{
    [Export]
    public string name { get; set; } = "blah";

    [Export]
    public Texture2D Sprite;

    [Export]
    public ScentColor Color;

    [Export]
    public ScentSize Size;

    [Export]
    public ScentShape Shape;

    public ItemData() { }
}
