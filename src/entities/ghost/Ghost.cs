using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Timer = Godot.Timer;

public partial class Ghost : CharacterBody3D
{
    enum State
    {
        PATROL,
        CHASE,
    }

    State CurrentState;
    public required NavigationAgent3D NavAgent;
    float Speed = 5f;
    public required RayCast3D PlayerDetectionRay;
    float MaxPlayerDetectionRange = 20;
    public required Player Player;
    public List<Vector3> PatrolRoute = [];
    public required Timer TrailCreationTimer;

    int CurrPoint = 0;

    Curve3D TrailCurve = null!;
    public required Path3D TrailPath;
    public required Node3D TrailPoints;

    const float POINT_ROUNDING_THRESHOLD = 0.1f;
    const int MAX_TRAIL_POINTS = 10;

    public override void _Ready()
    {
        TrailCurve = GetNode<Path3D>("Trail").Curve;
        TrailPath = GetNode<Path3D>("Trail");
        TrailPoints = GetNode<Node3D>("TrailPoints");

        NavAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");
        PlayerDetectionRay = GetNode<RayCast3D>("RayCast3D");
        Player = GetNode<Player>("../Player");
        TrailCreationTimer = new Timer();
        AddChild(TrailCreationTimer);
        TrailCreationTimer.Timeout += UpdateTrailCurve;
        TrailCreationTimer.OneShot = false;
        TrailCreationTimer.Start(1);
        CurrentState = State.PATROL;
    }

    public void RenderTrail()
    {
        var point = new Sprite3D
        {
            Texture = GD.Load<Texture2D>("res://assets/sprites/rei.png"),
            Scale = new Vector3(.5f, .5f, .5f),
            TopLevel = true,
        };
        TrailPoints.AddChild(point);
        point.GlobalPosition = TrailCurve.GetPointPosition(TrailCurve.PointCount - 1);
    }

    public void UpdateTrailCurve()
    {
        TrailCurve.AddPoint(GlobalTransform.Origin);
        if (TrailCurve.PointCount >= MAX_TRAIL_POINTS)
        {
            TrailCurve.RemovePoint(0);
            TrailPoints.GetChild(0).QueueFree();
        }
        RenderTrail();
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
