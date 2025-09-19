using System;
using Godot;

namespace Scent;

public partial class SmellItem : Node3D
{
    [Export]
    public ItemData? Data;
    public GpuParticles3D emitter;

    public override void _Ready()
    {
        if (Data == null)
        {
            GD.PrintErr($"Item {this} missing ItemData Resource");
        }
        AddToGroup("items");
        emitter = GetNode<GpuParticles3D>("ItemSmellTrail");
        if (Data.Effect == ScentEffect.SHAPE)
            emitter.DrawPass1.SurfaceSetMaterial(0, Data.Shape);
        else if (Data.Effect == ScentEffect.COLOUR)
            ((ParticleProcessMaterial)emitter.ProcessMaterial).Color = Data.Color;
        else
        {
            ((ParticleProcessMaterial)emitter.ProcessMaterial).ScaleMax = Data.Size;
            ((ParticleProcessMaterial)emitter.ProcessMaterial).ScaleMin = Data.Size;
        }
    }
}
