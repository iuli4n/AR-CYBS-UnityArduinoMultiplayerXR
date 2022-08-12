using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EP_Path_Manager : AEffect4PluginManager
{
    [SerializeField]
    private EP_Path_Menu myMenu;
    [SerializeField]
    private EP_Path_Model myModel;
    //bool startEnabled = false;

    bool menuGUIOn;
    bool savedMenuGUIOn;

    //List<Vector3> pointList;
    //List<Vector3> rotList;
    public GameObject drawingPath;
    LineRenderer lineRenderer;
    int numListPoints;


    public bool refreshPoints = false;
    bool usingPts = false;

    float drawingToPointsSimplifyFactor = .0001f;

    //bool interpolateDiscretePoints = false;
    bool useDrawingPoints = false;
    //bool interpolated = false;
    int numPointsPerInterp = 15;
    //public bool renderDiscretePoints;
    //bool renderedPoints = false;
    //public GameObject renderPointMarker;
    //List<GameObject> renderPointMarkerList;

    //List<Vector3> origListPoints;
    List<Vector3> firstListPoints;
    List<Vector3> firstListRots;

    override public AEffect4PluginModel GetModel() { return myModel; }
    override public AEffect4PluginMenu GetMenu() { return myMenu; }

    override public void OnCreated()
    {
        // nothing to do
    }
    override public void OnEnable()
    {
        // nothing to do
    }
    override public void OnDisable()
    {
        // nothing to do
    }

    public override void ToggleMenuGUI()
    {
        menuGUIOn = !menuGUIOn;
    }

    public override string GetPluginName()
    {
        return "Path";
        
    }

    // Start is called before the first frame update
    void Awake()
    {
        firstListPoints = myModel.pointList;
        firstListRots = myModel.rotList;

        numListPoints = myModel.pointList.Count;

        menuGUIOn = false;
        savedMenuGUIOn = false;
    }

    private void OnDestroy()
    {
        myModel.pointList = firstListPoints;
        myModel.rotList = firstListRots;
    }

    // Update is called once per frame
    void Update()
    {
        if (savedMenuGUIOn != menuGUIOn)
        {
            savedMenuGUIOn = menuGUIOn;
            if (menuGUIOn)
            {
                myMenu.Activate();
                myMenu.OpenMenu(this.transform);
            }
            else { myMenu.HideMenu(); }
        }
        UpdatePointsList();      
    }

    void UpdatePointsList()
    {
        // TODO: SET refreshPoints = true when user changes positions of path points in the scene with their hand
        if (refreshPoints && !usingPts)
        {
            if (useDrawingPoints && drawingPath != null)
            {
                UpdateModelPosFromDrawing();
            }
            else
            {
                UpdateModelPosAndRotLists();
            }
            usingPts = true;
        }
        else if (!refreshPoints && usingPts)
        {
            usingPts = false;
        }
    }

    void UpdateModelPosAndRotLists()
    {
        List<Vector3> newList = new List<Vector3>();
        List<Vector3> newRotList = new List<Vector3>();

        // interpolate points from pointList into newList using numPointsPerInterp
        // use float.Parse(n.ToString()) because apparently that is how to convert an integer to a float?

        // TODO: REPLACE firstListPoints with the model version
        for (int i = 0; i < firstListPoints.Count - 1; i++)
        {
            for (int n = 0; n < numPointsPerInterp; n++)
            {
                Vector3 newPos = Vector3.Lerp(firstListPoints[i], firstListPoints[i + 1], float.Parse(n.ToString()) / numPointsPerInterp);
                newList.Add(newPos);
            }
        }

        // interpolate rotations from rotList into newRotList using numPointsPerInterp
        for (int i = 0; i < firstListRots.Count - 1; i++)
        {
            for (int n = 0; n < numPointsPerInterp; n++)
            {
                Vector3 newRot = Quaternion.Lerp(Quaternion.Euler(firstListRots[i]), Quaternion.Euler(firstListRots[i + 1]), float.Parse(n.ToString()) / numPointsPerInterp).eulerAngles;
                newRotList.Add(newRot);
            }
        }

        //TODO: check possible race condition problem, if myModel.postList is accessed below before numListPoints is updated?
        myModel.pointList = newList;
        myModel.rotList = newRotList;
        numListPoints = newList.Count;
    }

    void UpdateModelPosFromDrawing()
    {
        List<Vector3> pointList = new List<Vector3>();
        lineRenderer = drawingPath.transform.GetComponent<LineRenderer>();
        lineRenderer.Simplify(drawingToPointsSimplifyFactor);

        numListPoints = lineRenderer.positionCount;
        for (int i = 0; i < numListPoints; i++)
        {
            // use Transform.TransformPoint beecause we want to transform the lineRenderer's points to go from its local space to world space, and then back to the local space of our object
            pointList.Add(lineRenderer.GetPosition(i)); // convert the lineRenderer point positions to the local space of my object
        }
        myModel.pointList = pointList;
    }

    override public void RespondToNewValue(
        float currentSensorValue, float oldSensorValue,
        Vector3 baseLocalPos, Quaternion baseLocalRot, Vector3 baseLocalScale,
        ref float newSensorValue,
        ref Vector3 newLocalPos, ref Quaternion newLocalRot, ref Vector3 newLocalScale)
    {
        // this plugin should cause the object to move and/or rotate along a path
        // according to the sensor value coming in

        if (!myModel.isEnabled)
            return;
        newLocalPos = ConvertSensorValToListPoint(currentSensorValue);
        Vector3 oldLocalPos = ConvertSensorValToListPoint(oldSensorValue);

        if (myModel.rotateObjectToDirectionOfPath) // FORWARD
        {
            newLocalRot = Quaternion.LookRotation(oldLocalPos - newLocalPos);
        }
        else if (myModel.rotateObjectBetweenInterpolatedRots) // LIST OF POINTS
        {
            newLocalRot = Quaternion.Euler(ConvertSensorValToListRot(currentSensorValue));
        }
    }

    Vector3 ConvertSensorValToListPoint(float sensorVal)
    {
        int interpolatedIndex = Mathf.FloorToInt((sensorVal / 1025f) * numListPoints);
        return myModel.pointList[interpolatedIndex];
    }

    Vector3 ConvertSensorValToListRot(float sensorVal)
    {
        int interpolatedIndex = Mathf.FloorToInt((sensorVal / 1025f) * numListPoints);
        return myModel.rotList[interpolatedIndex];
    }

    public void RespondToMenuToggle()
    {
        myMenu.Toggle();
        myModel.isEnabled = !myModel.isEnabled;
    }

    public void RespondToMenuPointsList()
    {
        myMenu.SetPosModeList();
        useDrawingPoints = false;
        UpdateModelPosAndRotLists();
    }

    public void RespondToMenuPointsDrawing()
    {
        myMenu.SetPosModeDrawing();
        useDrawingPoints = true;
        if (myModel.rotateObjectBetweenInterpolatedRots) { RespondToMenuRotationNone(); }
        UpdateModelPosFromDrawing();
    }

    public void RespondToMenuRotationNone()
    {
        myMenu.SetRotModeNone();
        myModel.rotateObjectBetweenInterpolatedRots = false;
        myModel.rotateObjectToDirectionOfPath = false;
    }

    public void RespondToMenuRotationForward()
    {
        myMenu.SetRotModeForward();
        myModel.rotateObjectBetweenInterpolatedRots = false;
        myModel.rotateObjectToDirectionOfPath = true;
    }

    public void RespondToMenuRotationList()
    {
        myMenu.SetRotModeList();
        myModel.rotateObjectBetweenInterpolatedRots = true;
        myModel.rotateObjectToDirectionOfPath = false;
    }

    public void RespondToMenuSliderValue()
    {
        float v = myMenu.slider.SliderValue;
        numPointsPerInterp = Mathf.RoundToInt(v * 30 + 1);
        UpdateModelPosAndRotLists();
        myMenu.SetValText(v);
    }
}
