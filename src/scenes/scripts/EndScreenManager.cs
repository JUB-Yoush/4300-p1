using System;
using Godot;

public partial class EndScreenManager : Node
{
    TextureButton mainBtn;
    TextureButton exitBtn;
    AnimationPlayer animPlayer;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Visible;
        mainBtn = GetNode<TextureButton>("HBoxContainer/Main");
        exitBtn = GetNode<TextureButton>("HBoxContainer/Exit");

        mainBtn.Disabled = true;
        exitBtn.Disabled = true;

        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animPlayer.Play("end_screen");
        animPlayer.AnimationFinished += AnimPlayer_AnimationFinished;

        exitBtn.Pressed += OnExitPressed;
        mainBtn.Pressed += OnMainPressed;
    }

    private void AnimPlayer_AnimationFinished(StringName animName)
    {
        GD.Print("Animation done");
        if (animName == "end_screen")
        {
            mainBtn.Disabled = false;
            exitBtn.Disabled = false;
        }
    }

    private void OnExitPressed()
    {
        GetTree().Quit();
    }

    private void OnMainPressed()
    {
        GetTree().ChangeSceneToFile("res://src/scenes/title.tscn");
    }
}
