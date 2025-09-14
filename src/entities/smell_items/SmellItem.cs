using System;
using Godot;

namespace Scent;

public partial class SmellItem : Node3D
{
    [Export]
    public ItemData? Data;
    GpuParticles3D emitter;

    public override void _Ready()
    {
        if (Data == null)
        {
            GD.PrintErr($"Item {this} missing ItemData Resource");
        }
        AddToGroup("items");
        GetNode<Sprite3D>("Sprite3D").Texture = Data!.Sprite;
        emitter = GetNode<GpuParticles3D>("ItemSmellTrail");
        if (Data.Shape != null)
            emitter.DrawPass1.SurfaceSetMaterial(0, Data.Shape);
        else if (Data.Color != Color.Color8(0, 0, 0, 0))
            ((ParticleProcessMaterial)emitter.ProcessMaterial).Color = Data.Color;
        else{
            ((ParticleProcessMaterial)emitter.ProcessMaterial).ScaleMax = Data.Size;
            ((ParticleProcessMaterial)emitter.ProcessMaterial).ScaleMin = Data.Size;
        }
    }
}
