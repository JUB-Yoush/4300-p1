using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Godot;

public partial class Main : Node
{
    Player Player;
    Ghost Ghost;
    List<Vector3> GhostPatrolPoints = [];
    int CurrPoint = 0;

    public override void _Ready()
    {
        Player = GetNode<Player>("Player");
        Ghost = GetNode<Ghost>("Ghost");
        var GhostPatrolCurve = GetNode<Path3D>("Level/Path3D").Curve;
        for (int i = 0; i < GhostPatrolCurve.PointCount; i++)
        {
            GhostPatrolPoints.Add(GhostPatrolCurve.GetPointIn(i));
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Ghost.FoundPlayer)
        {
            Ghost.UpdateTargetLocation(Player.GlobalTransform.Origin);
        }
        else
        {
            if (Ghost.GlobalTransform.Origin == GhostPatrolPoints[CurrPoint])
            {
                CurrPoint += 1 % GhostPatrolPoints.Count;
            }
            Ghost.UpdateTargetLocation(GhostPatrolPoints[CurrPoint]);
        }
    }
}
