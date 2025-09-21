using Godot;
using Scent;
using System;

public partial class PickupCollider : Area3D
{
    public override void _Ready()
    {
        Connect("body_entered", new Callable(this, "_OnBodyEntered"));
        Connect("body_exited", new Callable(this, "_OnBodyExit"));
    }

    public void _OnBodyEntered(Node3D body)
    {
        if (body.Name == "Player")
        {
            //TODO make pickup popup visible
            ((Player)body).TargetingItem = GetParent<SmellItem>();
        }
    }

    public void _OnBodyExit(Node3D body)
    {
        if (body.Name == "Player")
        {
            //TODO make pickup popup invisible
            ((Player)body).TargetingItem = null;
        }
    }
}
