using System;
using Godot;

public partial class SmellTrailArea : Area3D
{
    float SmellValue = 10;

    Player player = null!;
    Timer DecreaseTimer = null!;
    const float SCENT_LINGER_TIME = 15f;

    public override void _Ready()
    {
        player = GetParent().GetParent().GetParent().GetNode<Player>("Player");
        BodyEntered += Sniffed;
        DecreaseTimer = new Timer();
        AddChild(DecreaseTimer);
        DecreaseTimer.Start(SCENT_LINGER_TIME);
        DecreaseTimer.Timeout += () => QueueFree();
    }

    private void Sniffed(Node3D body)
    {
        player.UpdateSmellScore(5);
        QueueFree();
    }
}
