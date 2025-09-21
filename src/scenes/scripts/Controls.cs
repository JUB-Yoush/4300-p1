using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public partial class Controls : Node
{
    //GetTree().Paused = true;
    //Panel preGame;
    public static PackedScene packedScene = GD.Load<PackedScene>("res://src/scenes/controls.tscn");

    public override void _Ready()
    {
        GetTree().Paused = true;
        Input.MouseMode = Input.MouseModeEnum.Visible;

        var ResumeBtn = GetNode<TextureButton>("Paused/Panel/Resume");
        ResumeBtn.Pressed += UnPause;
        var exitBtn = GetNode<TextureButton>("Paused/Panel/Home");
        exitBtn.Pressed += () => GetTree().ChangeSceneToFile("res://src/scenes/title.tscn");
    }

    public void UnPause()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        GetTree().Paused = false;
        QueueFree();
    }
}
