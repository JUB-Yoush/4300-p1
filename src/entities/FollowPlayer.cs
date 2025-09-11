using Godot;
using System;

public partial class FollowPlayer : GpuParticles3D
{
    public override void _PhysicsProcess(double delta)
    {
        GlobalPosition = GetParent().GetNode<Node3D>("Player").GlobalPosition;
    }
}
