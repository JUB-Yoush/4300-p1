using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Godot.NativeInterop;
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
    Timer ContinueLookingTimer = null!;

    int CurrPoint = 0;

    Curve3D TrailCurve = null!;
    public required Path3D TrailPath;
    public required Node3D TrailPoints;
    public required CollisionPolygon3D TrailCollider;
    private List<Vector2> TrailBoxPoints = [];
    private List<Vector2> TrailBoxPointsLeft = [];
    private List<Vector2> TrailBoxPointsRight = [];

    const float POINT_ROUNDING_THRESHOLD = 0.1f;
    const int MAX_TRAIL_POINTS = 10;

    public override void _Ready()
    {
        TrailCurve = GetNode<Path3D>("Trail").Curve;
        TrailPath = GetNode<Path3D>("Trail");
        TrailPoints = GetNode<Node3D>("TrailPoints");
        TrailCollider = GetNode<CollisionPolygon3D>("GhostSmellTrail/SmellBox/TrailCollider");

        NavAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");
        PlayerDetectionRay = GetNode<RayCast3D>("RayCast3D");
        Player = GetNode<Player>("../Player");
        TrailCreationTimer = new Timer();
        ContinueLookingTimer = new Timer();
        AddChild(TrailCreationTimer);
        AddChild(ContinueLookingTimer);
        TrailCreationTimer.Timeout += UpdateTrailBox;
        ContinueLookingTimer.Timeout += SetToPatrol;

        TrailCreationTimer.OneShot = false;
        TrailCreationTimer.Start(1);
        CurrentState = State.PATROL;
    }

    public void SetToPatrol()
    {
        CurrentState = State.PATROL;
        UpdateTargetLocation(PatrolRoute[GetClosestPointIndex()]);
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

    public void UpdateTrailBox()
    {
        var direction = Velocity.Normalized();
        TrailBoxPointsLeft.Add(
            new Vector2(GlobalTransform.Origin.X, GlobalTransform.Origin.Z)
                + new Vector2(-direction.Z, direction.X).Normalized()
        );
        TrailBoxPointsRight.Add(
            new Vector2(GlobalTransform.Origin.X, GlobalTransform.Origin.Z)
                + new Vector2(direction.Z, -direction.X).Normalized()
        );

        if (TrailBoxPointsLeft.Count >= MAX_TRAIL_POINTS)
        {
            TrailBoxPointsLeft.RemoveAt(0);
            TrailBoxPointsRight.RemoveAt(0);
        }

        TrailBoxPoints = new List<Vector2>();
        for (int i = 0; i < TrailBoxPointsLeft.Count; i++)
            TrailBoxPoints.Add(TrailBoxPointsLeft[i]);
        for (int i = TrailBoxPointsRight.Count - 1; i >= 0; i--)
            TrailBoxPoints.Add(TrailBoxPointsRight[i]);
        TrailCollider.Polygon = null;
        TrailCollider.Polygon = [.. TrailBoxPoints];
    }

    public override void _PhysicsProcess(double delta)
    {
        GD.Print(CurrentState);
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
                        if (ContinueLookingTimer.IsStopped())
                        {
                            GD.Print("looking");
                            ContinueLookingTimer.Start(2);
                        }
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
