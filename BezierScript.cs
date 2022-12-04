using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierScript : MonoBehaviour
{

    public int numberControlPoints = 0;
    private int oldNumberControlPoints = 0;

    public GameObject controlPointObj;
    public GameObject linePointObj;


    public Vector3 spawnCenter;

    private float maxTime = 1f;
    private float currTime = 0f;

    private Vector3[] controlPoints;

    // Start is called before the first frame update
    void Start()
    {
        InitializeControlPositions();
        ClearPoints();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 bezierPos = CalculateBezier(numberControlPoints - 1, 0, currTime);

        transform.position = bezierPos;

        currTime = (currTime + 0.01f) % maxTime;
    }

    public void SpawnPoints(int type) {
        GameObject toAddObj;
        string namePrefix;
        if(type == 0) {
            toAddObj = controlPointObj;
            namePrefix = "P_";

        } else {
            toAddObj = linePointObj;
            namePrefix = "L_";
        }

        if(transform.Find("P_0") && oldNumberControlPoints == numberControlPoints) {
            // if control points exist already, randomize their position
            for(int i = 0; i < transform.childCount; i++) {
                if(transform.GetChild(i).gameObject.name.Contains("P")) {
                    transform.GetChild(i).transform.position = spawnCenter + new Vector3(Random.Range(0, 5), Random.Range(0, 5), Random.Range(0, 5));
                }
            }

        } else {
            if(oldNumberControlPoints != numberControlPoints && oldNumberControlPoints != 0) {
                // only adjust difference in controlPointsCount
                int difference = numberControlPoints -oldNumberControlPoints;

                if(difference > 0) {
                    for(int i = oldNumberControlPoints; i <= oldNumberControlPoints + difference - 1; i++) {
                        GameObject controlPoint = Instantiate(toAddObj, spawnCenter + new Vector3(Random.Range(0, 5), Random.Range(0, 5), Random.Range(0, 5)), Quaternion.identity);
                        controlPoint.transform.parent = transform;
                        controlPoint.name = namePrefix + i.ToString();
                    }

                } else {
                    for(int i = oldNumberControlPoints - 1; i > oldNumberControlPoints + difference - 1; i--) {
                        DestroyImmediate(transform.Find(namePrefix + i.ToString()).gameObject);
                    }
                }

            } else {
                // if no control points exist, create new ones
                for(int i = 0; i < numberControlPoints; i++) {
                    GameObject controlPoint = Instantiate(toAddObj, spawnCenter + new Vector3(Random.Range(0, 5), Random.Range(0, 5), Random.Range(0, 5)), Quaternion.identity);
                    controlPoint.transform.parent = transform;
                    controlPoint.name = namePrefix + i.ToString();
                }
            }
        }

        oldNumberControlPoints = numberControlPoints;
    }

    public void ClearPoints() {
        // TODO: find standard way to do so
        while(transform.childCount > 0) {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        oldNumberControlPoints = 0;
    }

    public void InitializeControlPositions() {
        if(controlPoints.Length != numberControlPoints) {
            controlPoints = new Vector3[numberControlPoints];
        }

        for(int i = 0; i < transform.childCount; i++) {
            if(transform.GetChild(i).gameObject.name.Contains("P")) {
                controlPoints[i] = transform.GetChild(i).position;
            }
        }
    }

    // Recursive De Casteljau for Bezier computation
    public Vector3 CalculateBezier(int i, int j, float t) {
        if(i == 0) {
            return controlPoints[j];
        }

        return (1 - t) * CalculateBezier(i - 1, j, t) + t * CalculateBezier(i - 1, j + 1, t);
    }

    // Build a Bezier Curve with GameObjects to show the user
    public void BuildBezier() {
        InitializeControlPositions();

        // TODO: move in SpawnPoints
        for(float t = 0f; t < maxTime; t += 0.1f) {
            GameObject linePoint = Instantiate(linePointObj, CalculateBezier(numberControlPoints - 1, 0, t), Quaternion.identity);
            linePoint.transform.parent = transform;
            linePoint.name = "L_" + (t * 10).ToString();
        }
    }
}
