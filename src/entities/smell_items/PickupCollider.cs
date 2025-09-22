using Godot;
using Scent;
using System;

public partial class PickupCollider : Area3D
{
    SmellItem item = null!;
    public override void _Ready()
    {
        Connect("body_entered", new Callable(this, "_OnBodyEntered"));
        Connect("body_exited", new Callable(this, "_OnBodyExit"));
        item = GetParent<SmellItem>();
    }

    public void _OnBodyEntered(Node3D body)
    {
        if (body.Name == "Player")
        {
            body.GetNode<RichTextLabel>("UI/Control/GameUi/RichTextLabel").Visible = true;
            body.GetNode<RichTextLabel>("UI/Control/GameUi/RichTextLabel").Text = "E to feed " + item.Data.name + " Into Gun (" + item.Data.Effect.ToString() + ").";
            ((Player)body).TargetingItem = item;
        }
    }

    public void _OnBodyExit(Node3D body)
    {
        if (body.Name == "Player")
        {
            body.GetNode<RichTextLabel>("UI/Control/GameUi/RichTextLabel").Visible = false;
            ((Player)body).TargetingItem = null;
        }
    }
}
