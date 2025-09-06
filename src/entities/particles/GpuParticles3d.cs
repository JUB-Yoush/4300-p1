using Godot;
using System;
using System.Collections;
using System.Threading.Tasks;

public partial class GpuParticles3d : GpuParticles3D
{
    [Export]
    private Material[] textures;
    [Export]
    private Material filledTexture;
    public async void _SimplifyColour()
    {
        ParticleProcessMaterial mat = GD.Load<ParticleProcessMaterial>(ProcessMaterial.ResourcePath);
        while (mat.HueVariationMax > 0)
        {
            await Task.Delay(20);
            mat.HueVariationMax -= 0.01f;
            mat.HueVariationMin += 0.01f;
        }
    }

    public async void _SimplifyShape()
    {
        for (int i = 0; i < textures.Length; i++)
        {
            await Task.Delay(1000);
            DrawPass1.SurfaceSetMaterial(0, textures[i]);
        }
    }

    public void _SimplifyFill()
    {
        DrawPass1.SurfaceSetMaterial(0, filledTexture);
    }
}
