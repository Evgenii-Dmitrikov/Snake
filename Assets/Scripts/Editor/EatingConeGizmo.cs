using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

[CustomEditor(typeof(SnakeHead))]
public class EatingConeGizmo : Editor
{
	void OnSceneGUI()
	{
		SnakeHead head = (SnakeHead)target;
		Handles.color = Color.white;
		Handles.DrawWireArc(head.transform.position, Vector3.up, Vector3.forward, 360, head.EatRadius);
		Vector3 viewAngleLeft = head.DirFromAngle(-head.EatAngle / 2, false);
		Vector3 viewAngleRight = head.DirFromAngle(head.EatAngle / 2, false);

		Handles.color = Color.red;
		Handles.DrawLine(head.transform.position, head.transform.position + viewAngleLeft * head.EatRadius);
		Handles.color = Color.green;
		Handles.DrawLine(head.transform.position, head.transform.position + viewAngleRight * head.EatRadius);
	}
}
