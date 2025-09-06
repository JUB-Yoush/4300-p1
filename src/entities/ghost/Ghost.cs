using System;
using System.Collections.Generic;
using System.Threading;
using Godot;

public partial class Ghost : CharacterBody3D
{
    enum State
    {
        PATROL,
        CHASE,
    }

    State CurrentState;
    NavigationAgent3D NavAgent;
    float Speed = 5f;
    public bool prevFoundPlayer;
    public bool FoundPlayer;
    RayCast3D PlayerDetectionRay;
    float MaxPlayerDetectionRange = 20;
    Player Player;
    public List<Vector3> PatrolRoute = [];

    int CurrPoint = 0;

    const float POINT_ROUNDING_THRESHOLD = 0.1f;

    public override void _Ready()
    {
        NavAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");
        PlayerDetectionRay = GetNode<RayCast3D>("RayCast3D");
        Player = GetNode<Player>("../Player");
        CurrentState = State.PATROL;
    }

    public override void _PhysicsProcess(double delta)
    {
        switch (CurrentState)
        {
            case State.PATROL:
                {
                    if (CanSeePlayer())
                    {
                        CurrentState = State.CHASE;
                        break;
                    }

                    if (
                        Math.Abs(GlobalTransform.Origin.Length() - PatrolRoute[CurrPoint].Length())
                        <= POINT_ROUNDING_THRESHOLD
                    )
                    {
                        CurrPoint = Mathf.Wrap(CurrPoint += 1, 0, PatrolRoute.Count - 1);
                    }
                    UpdateTargetLocation(PatrolRoute[CurrPoint]);
                }
                break;

            case State.CHASE:
                {
                    if (!CanSeePlayer())
                    {
                        CurrentState = State.PATROL;
                        UpdateTargetLocation(PatrolRoute[GetClosestPointIndex()]);
                        break;
                    }
                    UpdateTargetLocation(Player.GlobalTransform.Origin);
                }
                break;
        }
        Move();
    }

    public void Move()
    {
        var currentLocation = GlobalTransform.Origin;
        var nextLocation = NavAgent.GetNextPathPosition();
        var newVelocity = (nextLocation - currentLocation).Normalized() * Speed;
        Velocity = Velocity.MoveToward(newVelocity, 0.25f);
        MoveAndSlide();
    }

    public bool CanSeePlayer()
    {
        var playerVector = Player.GlobalTransform.Origin - GlobalTransform.Origin;
        PlayerDetectionRay.TargetPosition = playerVector;

        return PlayerDetectionRay.IsColliding()
            && PlayerDetectionRay.GetCollider().GetClass() == "CharacterBody3D" // TODO this should check for a player specific Class
            && playerVector.Length() <= MaxPlayerDetectionRange;
    }

    public void UpdateTargetLocation(Vector3 targetLocation)
    {
        NavAgent.TargetPosition = targetLocation;
    }

    public int GetClosestPointIndex()
    {
        int resIndex = -1;
        float resLen = 100_000_000;

        for (int i = 0; i < PatrolRoute.Count; i++)
        {
            var v = PatrolRoute[i];
            if (Math.Abs(v.Length() - GlobalTransform.Origin.Length()) < resLen)
            {
                resLen = Math.Abs(v.Length() - GlobalTransform.Origin.Length());
                resIndex = i;
            }
        }
        return resIndex;
    }
}
