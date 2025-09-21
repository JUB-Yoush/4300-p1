using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public partial class TitleManager : Node
{
    bool animationDone = false;
    AnimationPlayer animPlayer = null!;
    List<Button> buttons = [];
    Button playBtn = null!;
    Button creditsBtn;
    Button exitBtn;

    List<Panel> panels = new List<Panel>();

    Color standby = new Color(1, 1, 1, 1);
    Color hovered = new Color(1, 0, 0, 1);

    public override void _Ready()
    {
        playBtn = GetNode<Button>("CanvasLayer/Play/PlayButton");
        creditsBtn = GetNode<Button>("CanvasLayer/Credits/CreditsButton");
        exitBtn = GetNode<Button>("CanvasLayer/Exit/ExitButton");

        panels.Add(GetNode<Panel>("CanvasLayer/Play/Panel"));
        panels.Add(GetNode<Panel>("CanvasLayer/Credits/Panel"));
        panels.Add(GetNode<Panel>("CanvasLayer/Exit/Panel"));

        foreach (Panel panel in panels)
        {
            panel.Visible = false;
        }

        buttons.Add(playBtn);
        buttons.Add(creditsBtn);
        buttons.Add(exitBtn);

        foreach (Button button in buttons)
        {
            button.Disabled = true;
            _ButtonStandby(button);
        }

        animPlayer = GetNode<AnimationPlayer>("OpeningAnimation");
        animPlayer.Play("TitleAnimation");
        GD.Print("opening animation played");
        animPlayer.AnimationFinished += AnimPlayer_AnimationFinished;

        playBtn.Pressed += OnPlayPressed;
        creditsBtn.Pressed += OnCreditsPressed;
        exitBtn.Pressed += OnExitPressed;
    }

    private void OnPlayPressed()
    {
        GetTree().ChangeSceneToFile("res://src/toplevel_scenes/main.tscn");
    }

    private void OnCreditsPressed()
    {
        //TODO -- link to documents
    }

    private void OnExitPressed()
    {
        GetTree().Quit();
    }

    private void AnimPlayer_AnimationFinished(StringName animName)
    {
        GD.Print($"Animation '{animName}' finished!");
        if (animName == "TitleAnimation")
        {
            foreach (BaseButton button in buttons)
            {
                //animation done, we can have the buttons be clickable
                button.Disabled = false;
                animationDone = true;
            }
        }
    }

    public override void _Process(double delta)
    {
        foreach (Button button in buttons)
        {
            if (button.IsHovered() && animationDone)
            {
                _ButtonHovered(button);
            }
            else
            {
                _ButtonStandby(button);
            }
        }
    }

    public void _ButtonHovered(Button btn)
    {
        btn.SelfModulate = hovered;
        int index = buttons.IndexOf(btn);
        panels[index].Visible = true;
    }

    public void _ButtonStandby(Button btn)
    {
        btn.SelfModulate = standby;
        int index = buttons.IndexOf(btn);
        panels[index].Visible = false;
    }
}
