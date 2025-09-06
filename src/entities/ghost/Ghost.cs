using System;
using System.Threading;
using Godot;

public partial class Ghost : CharacterBody3D
{
    NavigationAgent3D NavAgent;
    float Speed = 5f;
    public bool FoundPlayer;
    RayCast3D PlayerDetectionRay;
    float MaxPlayerDetectionRange = 20;
    Player Player;

    public override void _Ready()
    {
        NavAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");
        PlayerDetectionRay = GetNode<RayCast3D>("RayCast3D");
        Player = GetNode<Player>("../Player");
    }

    public override void _PhysicsProcess(double delta)
    {
        FoundPlayer = FindPlayer();

        var currentLocation = GlobalTransform.Origin;
        var nextLocation = NavAgent.GetNextPathPosition();
        var newVelocity = (nextLocation - currentLocation).Normalized() * Speed;
        Velocity = Velocity.MoveToward(newVelocity, 0.25f);
        MoveAndSlide();
    }

    public bool FindPlayer()
    {
        // shoot a ray between the ghost and player, check if the ray can hit the player (line of sight) and that it's shorter than the max distance

        var playerVector = Player.GlobalTransform.Origin - GlobalTransform.Origin;
        PlayerDetectionRay.TargetPosition = playerVector;

        // GD.Print(playerVector.Length());

        // GD.Print(
        //     PlayerDetectionRay.IsColliding(),
        //     PlayerDetectionRay.GetCollider().GetClass(),
        //     playerVector.Length() <= MaxPlayerDetectionRange
        // );

        return PlayerDetectionRay.IsColliding()
            && PlayerDetectionRay.GetCollider().GetClass() == "CharacterBody3D"
            && playerVector.Length() <= MaxPlayerDetectionRange;
    }

    public void UpdateTargetLocation(Vector3 targetLocation)
    {
        NavAgent.TargetPosition = targetLocation;
    }
}
