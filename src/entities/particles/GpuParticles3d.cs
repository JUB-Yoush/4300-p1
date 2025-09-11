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
    [Export]
    private Color trueColor;
    public async void _SimplifyColour()
    {
        ParticleProcessMaterial mat = GD.Load<ParticleProcessMaterial>(ProcessMaterial.ResourcePath);
        while (mat.Color.R < trueColor.R)
        {
            await Task.Delay(20);
            mat.Color = new Color(mat.Color.R + (trueColor.R / 100), mat.Color.G + (trueColor.G / 100), mat.Color.B + (trueColor.B / 100), 1);
        }
    }

    //Hue Variation Color Mode//
    // public async void _SimplifyColour()
    // {
    //     ParticleProcessMaterial mat = GD.Load<ParticleProcessMaterial>(ProcessMaterial.ResourcePath);
    //     while (mat.HueVariationMax > 0)
    //     {
    //         await Task.Delay(20);
    //         mat.HueVariationMax -= 0.01f;
    //         mat.HueVariationMin += 0.01f;
    //     }
    // }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
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
    
    public async void _SimplifySize()
    {
        ParticleProcessMaterial mat = GD.Load<ParticleProcessMaterial>(ProcessMaterial.ResourcePath);
        while (mat.ScaleMax > 1.5f)
        {
            await Task.Delay(20);
            mat.ScaleMax -= 0.01f;
            mat.ScaleMin += 0.01f;
        }
    }
}
