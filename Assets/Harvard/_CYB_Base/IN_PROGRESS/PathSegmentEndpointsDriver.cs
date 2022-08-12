using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathSegmentEndpointsDriver : MonoBehaviour
{
    //public GameObject startObject, endObject;
    public List<Vector3> pointList;
    public PathSegment pathSegment;
    //public GameObject pointPrefab;
    //List<GameObject> pointMarkers = new List<GameObject>();


    public AtomicDataSwitch speedModel;
    public float speedModelScaling = 1;
    public bool speedModelScalingExponential = false;

    public AtomicDataSwitch amountModel;
    public float amountModelScaling = 1;
    public bool amountModelScalingExponential = false;


    public GameObject childrenPointsContainer;
    public bool getPointsFromChild;
    List<Vector3> GetPointsFromChildren(GameObject childrenPoints)
    {
        List<Vector3> childPositions = new System.Collections.Generic.List<Vector3>();

        foreach (Transform t in childrenPoints.transform)
        {
            childPositions.Add(t.position);
        }
        return childPositions;
    }




    void Awake()
    {
        pathSegment.SetNumSegments(pointList.Count - 1);
        pathSegment.SetPointList(pointList);
    }

    // Start is called before the first frame update
    void Start()
    {
        /*
        foreach(Vector3 p in pointList)
        {
            GameObject go = Instantiate(pointPrefab, Vector3.zero, Quaternion.identity);
            go.SetActive(true);
            go.transform.position = p;
            pointMarkers.Add(go);
        }
        */

        if (speedModel) speedModel.OnDataUpdated += OnSpeedUpdated;
        if (amountModel) amountModel.OnDataUpdated += OnAmountUpdated;
    }

    private void OnSpeedUpdated(float newsensor)
    {
        pathSegment.SetSpeed(Mathf.Pow(newsensor / 1024f * 0.05f * speedModelScaling, speedModelScalingExponential ? 2 : 1));
    }
    private void OnAmountUpdated(float newsensor)
    {
        pathSegment.NUMELEMENTS = (int)Mathf.Pow(newsensor / 10 * amountModelScaling, amountModelScalingExponential ? 2 : 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (getPointsFromChild)
        {
            pointList = GetPointsFromChildren(childrenPointsContainer);
        }


        pathSegment.SetNumSegments(pointList.Count - 1);
        pathSegment.SetPointList(pointList);
        pathSegment.startPos = pointList[0];
        pathSegment.endPos = pointList[pointList.Count - 1];

        /*
        for (int i = 0; i < pointList.Count; i++)
        {
            if (pointMarkers[i].transform.position != pointList[i])
            {
                pointList[i] = pointMarkers[i].transform.position;
            }
        }
        */
    }
}
