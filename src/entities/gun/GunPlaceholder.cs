using System;
using System.Threading.Tasks;
using Godot;

public partial class GunPlaceholder : Node3D
{
    string bulletPath = "res://src/scenes/scent_projectile.tscn";
    PackedScene packedScene;
    public required AnimationPlayer AnimPlayer;

    [Export]
    private Material projectileMaterial;

    [Export]
    private Color projectileColour;

    [Export]
    private float power = 10;
    private BaseMaterial3D baseMaterial;
    private bool onCooldown;

    public override void _Ready()
    {
        //AnimPlayer = GetNode<AnimationPlayer>("../../AnimationPlayer");
        AnimPlayer = GetParent().GetParent().GetParent().GetNode<AnimationPlayer>("AnimPlayer");
        baseMaterial = ResourceLoader.Load<StandardMaterial3D>(
            "res://assets/Materials/BulletMaterialV1.tres",
            typeHint: "StandardMaterial3D",
            cacheMode: ResourceLoader.CacheMode.Ignore
        );
        packedScene = GD.Load<PackedScene>(bulletPath);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("shoot") && !onCooldown)
        {
            AnimPlayer.Play("fire_gun");
            RigidBody3D bullet = packedScene.Instantiate<RigidBody3D>();
            AddChild(bullet); //throws node not found error but still works
            bullet.TopLevel = true;
            GpuParticles3D emitter = bullet.GetChild<GpuParticles3D>(0);
            emitter.DrawPass1.SurfaceSetMaterial(0, projectileMaterial);
            ((ParticleProcessMaterial)emitter.ProcessMaterial).Color = projectileColour;
            bullet.GetChild<MeshInstance3D>(2).SetSurfaceOverrideMaterial(0, baseMaterial);
            ((StandardMaterial3D)bullet.GetChild<MeshInstance3D>(2).MaterialOverride).AlbedoColor =
                projectileColour;
            bullet.LinearVelocity =
                GetParent().GetParent().GetParent<CharacterBody3D>().Velocity / 2; //lmao
            bullet.ApplyCentralImpulse(-GlobalBasis.Z * power);
            _Cooldown();
        }
    }

    private async void _Cooldown()
    {
        onCooldown = true;
        await Task.Delay(1000);
        onCooldown = false;
    }
}
