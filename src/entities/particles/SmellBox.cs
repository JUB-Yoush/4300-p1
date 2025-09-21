using Godot;
using System;

public partial class SmellBox : Area3D
{
    private bool smelling = false;
    public double smellLevel = 0;
    public int smellStage = 0;
    public override void _Ready()
    {
        Connect("body_entered", new Callable(this, "_OnBodyEntered"));
        Connect("body_exited", new Callable(this, "_OnBodyExit"));
    }

    public override void _PhysicsProcess(double delta)
    {
        if (smelling)
            smellLevel += delta;

        if (smellLevel > 2 && smellStage == 0)
        {
            GD.Print("Simplifying Color");
            GetParent<GpuParticles3d>()._SimplifyColour();
            smellStage = 1;
        }
        else if (smellLevel > 6 && smellStage == 1)
        {
            GD.Print("Simplifying Fill");
            //GetParent<GpuParticles3d>()._SimplifyFill();
            GetParent<GpuParticles3d>()._SimplifySize();
            smellStage = 2;
        }
        else if (smellLevel > 10 && smellStage == 2)
        {
            GD.Print("Simplifying Shape");
            GetParent<GpuParticles3d>()._SimplifyShape();
            smellStage = 3;
        }

    }

    public void _OnBodyEntered(Node3D body)
    {
        if (body.Name == "Player")
        {
            GD.Print("Smelling");
            smelling = true;
        }
    }

    public void _OnBodyExit(Node3D body)
    {
        if (body.Name == "Player")
        {
            GD.Print("Not Smelling");
            smelling = false;
        }
    }
}
