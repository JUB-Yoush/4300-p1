using System;
using Godot;

public partial class SmellTrailArea : Area3D
{
    float SmellValue = 10;
    Timer DecreaseTimer = null!;
    const float SCENT_LINGER_TIME = 9f;
    public bool smelling;

    public override void _Ready()
    {
        AddToGroup("SmellBoxes");
        BodyEntered += Sniffed;
        BodyExited += UnSniffed;
        DecreaseTimer = new Timer();
        AddChild(DecreaseTimer);
        DecreaseTimer.Start(SCENT_LINGER_TIME);
        DecreaseTimer.Timeout += QueueFree;
    }

    private void Sniffed(Node3D body)
    {
        if (body.Name == "Player")
            smelling = true;
    }

    private void UnSniffed(Node3D body)
    {
        if (body.Name == "Player")
            smelling = false;
    }
}
