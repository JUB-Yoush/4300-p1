using System;
using Godot;

public partial class SmellTrailArea : Area3D
{
    float SmellValue = 10;

    Player player = null!;
    Timer DecreaseTimer = null!;

    public override void _Ready()
    {
        player = GetParent().GetParent().GetParent().GetNode<Player>("Player");
        AreaEntered += Sniffed;
    }

    private void Sniffed(Area3D area)
    {
        player.UpdateSmellScore(5);
        QueueFree();
    }
}
