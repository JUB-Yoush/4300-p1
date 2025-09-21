using Godot;
using System;
using System.Collections;
using System.Threading.Tasks;

public partial class GpuParticles3d : GpuParticles3D
{
    [Export]
    public Material[] textures;
    [Export]
    private Material filledTexture;
    [Export]
    public Color trueColor;
    private bool smelling = false;
    public double smellLevel = 0;
    public int smellStage = 0;
    Player player = null!;

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

    public override void _Ready()
    {
        player = GetParent().GetParent().GetNode<Player>("Player");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (smellLevel == 3) return;

        smelling = false;

        foreach (Node obj in GetTree().GetNodesInGroup("SmellBoxes"))
        {
            if (((SmellTrailArea)obj).smelling)
            {
                smelling = true;
                break;
            }
        }

        if (smelling)
            smellLevel += delta;

        if (smellLevel > 3.33 && smellStage == 0)
        {
            GD.Print("Simplifying Color");
            _SimplifyColour();
            smellStage = 1;
        }
        else if (smellLevel > 6.66f && smellStage == 1)
        {
            GD.Print("Simplifying Fill");
            //GetParent<GpuParticles3d>()._SimplifyFill();
            _SimplifySize();
            smellStage = 2;
        }
        else if (smellLevel > 10 && smellStage == 2)
        {
            GD.Print("Simplifying Shape");
            _SimplifyShape();
            smellStage = 3;
        }
        player.UpdateSmellScore((float)smellLevel * 10);

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
        while (mat.ScaleMin < 2.5f)
        {
            await Task.Delay(20);
            mat.ScaleMin += 0.01f;
        }
        mat.ScaleMax = 2.5f;
        mat.ScaleMin = 2.5f;
    }
}
