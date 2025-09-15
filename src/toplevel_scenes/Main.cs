using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading;
using Godot;

public partial class Main : Node
{
	Ghost Ghost;
	int CurrPoint = 0;

	public override void _Ready()
	{
		List<Vector3> GhostPatrolPoints = [];
		Ghost = GetNode<Ghost>("Ghost");
		var GhostPatrolCurve = GetNode<Path3D>("Level/Path3D").Curve;
		for (int i = 0; i < GhostPatrolCurve.PointCount; i++)
		{
			GhostPatrolPoints.Add(GhostPatrolCurve.GetPointPosition(i));
		}
		Ghost.PatrolRoute = GhostPatrolPoints;
	}
}
