using System;
using System.Threading.Tasks;
using Godot;

public partial class ScentProjectile : RigidBody3D
{
    public override void _Ready()
    {
        AddToGroup("bullets");
        _Despawn();
    }

    public override void _Process(double delta)
    {
        GetNode<GpuParticles3D>("BulletSmellTrail").Visible = GetParent()
            .GetNode<Player>("Player")
            .smelling;
    }

    private async void _Despawn()
    {
        await Task.Delay(10000);
        StandardMaterial3D mat = (StandardMaterial3D)GetChild<MeshInstance3D>(2).MaterialOverride;
        GetChild<GpuParticles3D>(0).Emitting = false;
        for (float a = 255; a > 0; a -= 4)
        {
            mat.AlbedoColor = new Color(
                mat.AlbedoColor.R,
                mat.AlbedoColor.G,
                mat.AlbedoColor.B,
                a / 255
            );
            await Task.Delay(10);
        }
        await Task.Delay(3000);
        QueueFree();
    }
}
