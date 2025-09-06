using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading;
using Godot;

public partial class Main : Node
{
    Player Player;
    Ghost Ghost;
    List<Vector3> GhostPatrolPoints = [];
    int CurrPoint = 0;
    const float CLOSE_ENOUGH_TO_POINT = 0.1f;

    public override void _Ready()
    {
        Player = GetNode<Player>("Player");
        Ghost = GetNode<Ghost>("Ghost");
        var GhostPatrolCurve = GetNode<Path3D>("Level/Path3D").Curve;
        for (int i = 0; i < GhostPatrolCurve.PointCount; i++)
        {
            GhostPatrolPoints.Add(GhostPatrolCurve.GetPointPosition(i));
            GD.Print(GhostPatrolPoints[i]);
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
            GD.Print(Ghost.GlobalTransform.Origin.Length() - GhostPatrolPoints[CurrPoint].Length());
            //GD.Print(Ghost.GlobalTransform.Origin.IsEqualApprox(GhostPatrolPoints[CurrPoint]));
            if (
                Math.Abs(
                    Ghost.GlobalTransform.Origin.Length() - GhostPatrolPoints[CurrPoint].Length()
                ) <= CLOSE_ENOUGH_TO_POINT
            //Ghost.GlobalTransform.Origin.IsEqualApprox(GhostPatrolPoints[CurrPoint])
            )
            {
                CurrPoint = Mathf.Wrap(CurrPoint += 1, 0, GhostPatrolPoints.Count - 1);
            }
            Ghost.UpdateTargetLocation(GhostPatrolPoints[CurrPoint]);
        }
    }

    public void LostPlayer()
    {
        CurrPoint = GetClosestPointIndex();
    }

    public int GetClosestPointIndex()
    {
        int resIndex = -1;
        float resLen = 100_000_000;

        for (int i = 0; i < GhostPatrolPoints.Count; i++)
        {
            var v = GhostPatrolPoints[i];
            if (Math.Abs(v.Length() - Ghost.GlobalTransform.Origin.Length()) < resLen)
            {
                resLen = Math.Abs(v.Length() - Ghost.GlobalTransform.Origin.Length());
                resIndex = i;
            }
        }
        return resIndex;
    }
}
