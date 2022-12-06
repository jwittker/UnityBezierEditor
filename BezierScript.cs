using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierScript : MonoBehaviour
{

    public int numberControlPoints = 2;
    private int oldNumberControlPoints = 2;

    public GameObject controlPointObj;
    public GameObject linePointObj;


    public Vector3 spawnCenter;

    private float maxTime = 1f;
    private float currTime = 0f;

    private Vector3[] controlPoints = new Vector3[2];

    private bool validControlPoints;
    private bool controlPointsChanged = true;
    private bool showBezier = true;


    // Start is called before the first frame update
    void Start()
    {
        showBezier = false;
        validControlPoints = InitializeControlPositions();

        ClearPoints(2);
    }

    // Update is called once per frame
    void Update()
    {
        if(!validControlPoints) return;

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

        Transform foundTransform = transform.Find("P_0");
        if(foundTransform && oldNumberControlPoints == numberControlPoints) {
            // if control points exist already, randomize their position
            for(int i = 0; i < transform.childCount; i++) {
                if(transform.GetChild(i).gameObject.name.Contains("P")) {
                    transform.GetChild(i).transform.position = spawnCenter + new Vector3(Random.Range(0, 5), Random.Range(0, 5), Random.Range(0, 5));
                }
            }

        } else {
            if(oldNumberControlPoints != numberControlPoints && foundTransform) {
                // only adjust difference in controlPointsCount
                int difference = numberControlPoints - oldNumberControlPoints;

                if(difference > 0) {
                    for(int i = oldNumberControlPoints; i <= oldNumberControlPoints + difference - 1; i++) {
                        GameObject controlPoint = Instantiate(toAddObj, spawnCenter + new Vector3(Random.Range(0, 5), Random.Range(0, 5), Random.Range(0, 5)), Quaternion.identity);
                        controlPoint.transform.parent = transform;
                        controlPoint.name = namePrefix + i.ToString();
                    }

                } else {
                    for(int i = oldNumberControlPoints - 1; i > oldNumberControlPoints + difference - 1; i--) {
                        GameObject toDestroy = transform.Find(namePrefix + i.ToString()).gameObject;
                        if(toDestroy) {
                            DestroyImmediate(toDestroy);
                        }
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
        controlPointsChanged = true;
    }

    public void ClearPoints(int type) {
        Debug.Log("ClearPoints " + type.ToString());
        int childIdx = 0;
        int childCount = transform.childCount;
        while(childCount > childIdx) {
            if(type == 2 || (type == 0 && transform.GetChild(childIdx).name.Contains("P_")) || (type == 1 && transform.GetChild(childIdx).name.Contains("L_"))) {
                DestroyImmediate(transform.GetChild(childIdx).gameObject);
                childCount--;

            } else {
                childIdx++;
            }
        }

        // oldNumberControlPoints = 0;
    }

    public bool InitializeControlPositions() {
        if(transform.childCount == 0 || !transform.Find("P_0")) {
            return false;
        }

        if(controlPointsChanged) {
            controlPoints = new Vector3[numberControlPoints];
        }

        int checkSum = 0;
        for(int i = 0; i < controlPoints.Length; i++) {
            Transform controlTransform = transform.Find("P_" + i.ToString());
            if(controlTransform) {
                controlPoints[i] = controlTransform.position;
                checkSum++;
            }
        }

        controlPointsChanged = false;

        return checkSum == controlPoints.Length;
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
        if(!transform.Find("P_0")) {
            SpawnPoints(0);
        }
        bool check = InitializeControlPositions();

        if(!check) return;

        for(float t = 0f; t < maxTime; t += 0.1f) {
            Vector3 bezierPos = CalculateBezier(numberControlPoints - 1, 0, t);
            Transform lineTransform = transform.Find("L_" + Mathf.Floor(t * 10).ToString());
            if(lineTransform) {
                lineTransform.position = bezierPos;

            } else {
                GameObject linePoint = Instantiate(linePointObj, bezierPos, Quaternion.identity);
                linePoint.transform.parent = transform;
                linePoint.name = "L_" + Mathf.Floor(t * 10).ToString();
            }
        }

    }

    public int GetOldNumberControlPoints() {
        return oldNumberControlPoints;
    }

    public bool GetShowBezier() {
        return showBezier;
    }

    public void SetShowBezier(bool b) {
        showBezier = b;
    }
}
