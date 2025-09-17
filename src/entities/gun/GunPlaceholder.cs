using System;
using System.Drawing;
using System.Threading.Tasks;
using Godot;
using Scent;

public partial class GunPlaceholder : Node3D
{
    string bulletPath = "res://src/entities/scent_projectile/scent_projectile.tscn";

    PackedScene? packedScene;
    public required AnimationPlayer AnimPlayer;

    [Export]
    public Material? projectileMaterial;

    [Export]
    public Godot.Color projectileColour;

    [Export]
    private float power = 10;
    private BaseMaterial3D? baseMaterial;
    private bool onCooldown;
    public float projectileMinSize = 0.6f,
        projectileMaxSize = 2.4f;
    private GpuParticles3d? ghostParticles;

    public override void _Ready()
    {
        //AnimPlayer = GetNode<AnimationPlayer>("../../AnimationPlayer");
        AnimPlayer = GetParent()
            .GetParent()
            .GetParent()
            .GetNode<AnimationPlayer>("PlayerAnimation/AnimationPlayer");
        baseMaterial = ResourceLoader.Load<StandardMaterial3D>(
            "res://assets/Materials/BulletMaterialV1.tres",
            typeHint: "StandardMaterial3D"
        //cacheMode: ResourceLoader.CacheMode.Ignore
        );
        packedScene = GD.Load<PackedScene>(bulletPath);
        ghostParticles = GetParent()
            .GetParent()
            .GetParent()
            .GetParent().GetNode<GpuParticles3d>("Ghost/GhostSmellTrail");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("shoot") && !onCooldown)
        {
            AnimPlayer.Play("gun_shoot");
            _Shoot();
            _Cooldown();
        }
    }

    private void _Shoot()
    {
        var bullet = packedScene!.Instantiate<ScentProjectile>();
        bullet.TopLevel = true;
        GetParent().GetParent().GetParent().GetParent().AddChild(bullet); //throws node not found error but still works
        bullet.GlobalPosition = GlobalPosition;
        GpuParticles3D emitter = bullet.GetChild<GpuParticles3D>(0);
        emitter.DrawPass1.SurfaceSetMaterial(0, projectileMaterial);
        ((ParticleProcessMaterial)emitter.ProcessMaterial).Color = projectileColour;
        ((ParticleProcessMaterial)emitter.ProcessMaterial).ScaleMax = projectileMaxSize;
        ((ParticleProcessMaterial)emitter.ProcessMaterial).ScaleMin = projectileMinSize;

        bullet.GetChild<MeshInstance3D>(2).SetSurfaceOverrideMaterial(0, baseMaterial);
        ((StandardMaterial3D)bullet.GetChild<MeshInstance3D>(2).MaterialOverride).AlbedoColor =
            projectileColour;
        bullet.LinearVelocity = GetParent().GetParent().GetParent<CharacterBody3D>().Velocity / 2; //lmao
        bullet.ApplyCentralImpulse(-GlobalBasis.Z * power);
        if (projectileColour == ghostParticles.trueColor
            && projectileMaterial == ghostParticles.textures[2]
            && projectileMaxSize == 1.5f
            && projectileMinSize == 1.5f)
        {
            bullet.luresGhost = true;
        }
    }

    private async void _Cooldown()
    {
        onCooldown = true;
        await Task.Delay(1000);
        onCooldown = false;
    }
}
