using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierScript))]
public class NewBehaviourScript : Editor
{
    private int minControlPoints = 2;
    private int maxControlPoints = 10;

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        BezierScript myScript = (BezierScript)target;

        // limit number of control points
        myScript.numberControlPoints = Mathf.Clamp(myScript.numberControlPoints, minControlPoints, maxControlPoints);

        if(GUILayout.Button("Spawn Control Points")) {
            myScript.SpawnPoints(0);
        }

        if(GUILayout.Button("Clear Points")) {
            myScript.ClearPoints();
        }

        if(GUILayout.Button("Build Bezier")) {
            myScript.BuildBezier();
        }

    }
}
