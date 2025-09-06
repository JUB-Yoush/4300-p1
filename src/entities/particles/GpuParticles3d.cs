using Godot;
using System;
using System.Collections;
using System.Threading.Tasks;

public partial class GpuParticles3d : GpuParticles3D
{
    [Export]
    private Material[] textures;
    [Export]
    private Material finalTexture;
    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("jump"))
        {
            _SimplifyColour();
            _SimplifyShape();
            DrawPass1.SurfaceSetMaterial(0, finalTexture);
        }
    }

    private async void _SimplifyColour()
    {
        ParticleProcessMaterial mat = GD.Load<ParticleProcessMaterial>(ProcessMaterial.ResourcePath);
        while (mat.HueVariationMax > 0)
        {
            await Task.Delay(20);
            mat.HueVariationMax -= 0.01f;
            mat.HueVariationMin += 0.01f;
        }
    }

    private async void _SimplifyShape()
    {
        for (int i = 0; i < textures.Length; i++)
        {
            await Task.Delay(200);
            DrawPass1.SurfaceSetMaterial(0, textures[i]);
        }
    }
}
