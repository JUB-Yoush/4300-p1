using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
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
        SMELLING, //when you throw the correct scent out
    }

    PackedScene RigidGhost = null!;
    State CurrentState;
    public required NavigationAgent3D NavAgent;
    float Speed = 5f;
    public required RayCast3D PlayerDetectionRay;
    float MaxPlayerDetectionRange = 20;
    public required Player Player;
    public List<Vector3> PatrolRoute = [];
    public Vector3 CurrentTargetPosition = Vector3.Zero;
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

    public bool InHomeRoom = false;
    bool active = true;

    const float POINT_ROUNDING_THRESHOLD = 0.1f;
    const float TELEPORT_DIST_THRESHOLD = 25f;
    const float ATTACK_RANGE = 2f;
    const int MAX_TRAIL_POINTS = 10;

    public bool IsCorporial()
    {
        return InHomeRoom && CurrentState == State.SMELLING;
    }

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
        RigidGhost = GD.Load<PackedScene>("res://src/entities/ghost/rigid_ghost.tscn");

        TrailCreationTimer.OneShot = false;
        TrailCreationTimer.Start(1);
        CurrentState = State.PATROL;
        CurrentTargetPosition = GetNewPoint();
        var PunchHitbox = Player.GetNode<Area3D>("Head/PunchBox");
        var HomeRoom = GetParent().GetNode<Area3D>("Level/HomeRoom");
        PunchHitbox.AreaEntered += Punched;

        HomeRoom.AreaEntered += (area) => InHomeRoom = true;
        HomeRoom.AreaExited += (area) => InHomeRoom = false;
    }

    public void Punched(Area3D area)
    {
        if (!active)
        {
            return;
        }
        if (InHomeRoom && CurrentState == State.SMELLING)
        {
            var rigidGhost = RigidGhost.Instantiate<RigidBody3D>();
            GetParent().AddChild(rigidGhost);
            GetNode<Node3D>("ghost").Visible = false;
            var camera = Player.GetNode<Camera3D>("Head/Camera3D");
            var punchVelocity = camera.GlobalTransform.Basis.Z;
            active = false;
            rigidGhost.GlobalPosition = GlobalPosition;
            rigidGhost.LinearVelocity = -punchVelocity * 5;
        }
        else
        {
            GD.Print("ghost not dead as heck");
        }
    }

    public void SetToPatrol()
    {
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

    public bool ScentIsCorrect()
    {
        return true; // TODO implement
    }

    public void UpdateTrailBox()
    {
        if (!active)
        {
            return;
        }
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

    public void Teleport()
    {
        var newSpot = GetNewPoint();
        CurrentState = State.PATROL;
        GlobalPosition = newSpot;
        CurrPoint = PatrolRoute.IndexOf(newSpot);
        UpdateTargetLocation(GetNewPoint());
    }

    public override void _PhysicsProcess(double delta)
    {
        //GD.Print(CurrentState, GetTree().GetNodesInGroup("bullets").Count);
        //GD.Print(InHomeRoom);
        if (!active)
        {
            return;
        }

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
                        Math.Abs(GlobalPosition.Length() - CurrentTargetPosition.Length())
                        <= POINT_ROUNDING_THRESHOLD
                    )
                    {
                        CurrentTargetPosition = GetNewPoint();
                    }
                    UpdateTargetLocation(CurrentTargetPosition);
                }
                break;

            case State.CHASE:
                {
                    if (!CanSeePlayer())
                    {
                        if (ContinueLookingTimer.IsStopped())
                        {
                            ContinueLookingTimer.Start(2);
                        }
                        break;
                    }
                    if (Math.Abs((GlobalPosition - Player.GlobalPosition).Length()) <= ATTACK_RANGE)
                    {
                        Attack();
                        Teleport();
                    }
                    UpdateTargetLocation(Player.GlobalTransform.Origin);
                }
                break;

            case State.SMELLING:
                {
                    if (GetTree().GetNodesInGroup("bullets").Count == 0)
                    {
                        CurrentState = State.PATROL;
                        break;
                    }

                    UpdateTargetLocation(GetNearestBulletLocation());
                }
                break;
        }
        Move();
        if (GetTree().GetNodesInGroup("bullets").Count > 0 && ScentIsCorrect())
        {
            foreach (Node bullet in GetTree().GetNodesInGroup("bullets"))
            {
                if (((ScentProjectile)bullet).luresGhost)
                    CurrentState = State.SMELLING;
            }
        }

        LookAt(
            GlobalPosition
                + (GlobalPosition - NavAgent.TargetPosition with { Y = GlobalPosition.Y }),
            Vector3.Up
        );
    }

    public Vector3 GetNearestBulletLocation()
    {
        var bullets = GetTree().GetNodesInGroup("bullets").Cast<RigidBody3D>();
        return bullets
            .Select(bullet => bullet.GlobalPosition)
            .OrderBy(position => (GlobalPosition - position).Length())
            .First();
    }

    public void Attack()
    {
        GD.Print("you got slimed");
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

    public Vector3 GetNewPoint()
    {
        Random rng = new();
        var positions = GetParent().GetNode<Node3D>("Level/Positions");
        var newPos = positions
            .GetChild<Marker3D>(rng.Next(0, positions.GetChildCount()))
            .GlobalPosition;
        while (newPos == CurrentTargetPosition)
        {
            newPos = positions
                .GetChild<Marker3D>(rng.Next(0, positions.GetChildCount()))
                .GlobalPosition;
        }
        CurrentTargetPosition = newPos;
        return newPos;
    }
}
