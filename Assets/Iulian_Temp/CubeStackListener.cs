using System;
using TMPro;
using UnityEngine;

public class CubeStackListener : MonoBehaviour
{
    public GameObject particleEffects;

    public GameObject trackedObjectA, trackedObjectB;
    public GameObject XRight, XLeft;
    public GameObject YUp, YDown;
    public GameObject ZRight, ZLeft;
    public GameObject XPlane, YPlane, ZPlane;
    public Material redMaterial, greenMaterial;
    public MeshRenderer xPlaneBarMesh, yPlaneBarMesh, zPlaneBarMesh;
    public TextMeshPro xNotchAValue, xNotchBValue, yNotchAValue, yNotchBValue, zNotchAValue, zNotchBValue;
    public float offsetX, offsetZ, offsetYA, offsetYB;
    public float offsetXLeft, offsetXRight, offsetYUp, offsetYDown, offsetZLeft, offsetZRight;
    public GameObject[] images;
    public ProjectionManager xProjection, yProjection, zProjection;

    GameObject[] stateVisualMarkers;
    bool Right, Left, isXOverlap, Above, Below, isYOverlap, RightZ, LeftZ, isZOverlap;
    bool[] states;
    float diffXLeft, diffXRight, diffYUp, diffYDown, diffZLeft, diffZRight;

    // Start is called before the first frame update
    void Start()
    {
        stateVisualMarkers = new GameObject[]
        {
            XRight,
            XLeft,
            YUp,
            YDown,
            ZRight,
            ZLeft
        };

    }

    // Update is called once per frame
    void Update()
    {
        UpdateStates();
        UpdateVisuals();
    }

    void UpdateStates()
    {
        diffXRight = trackedObjectB.transform.localPosition.x - (trackedObjectA.transform.localPosition.x - offsetX);
        diffXLeft = trackedObjectA.transform.localPosition.x + offsetX - trackedObjectB.transform.localPosition.x;
        Right = diffXRight > 0;
        Left = diffXLeft > 0;
        isXOverlap = Right && Left;

        diffYUp = trackedObjectB.transform.localPosition.y - (trackedObjectA.transform.localPosition.y + offsetYA - offsetYB);
        diffYDown = trackedObjectA.transform.localPosition.y + offsetYA + offsetYB - trackedObjectB.transform.localPosition.y;
        Above = diffYUp > 0;
        Below = diffYDown > 0;
        isYOverlap = Above && Below;

        diffZRight = trackedObjectB.transform.localPosition.z - (trackedObjectA.transform.localPosition.z - offsetZ);
        diffZLeft = trackedObjectA.transform.localPosition.z + offsetZ - trackedObjectB.transform.localPosition.z;
        RightZ = diffZRight > 0;
        LeftZ = diffZLeft > 0;
        isZOverlap = RightZ && LeftZ;

        states = new bool[]
        {
            Right && !Left,
            Left && !Right,
            isXOverlap && Above && !Below,
            isXOverlap && Below && !Above,
            isXOverlap && isYOverlap && RightZ && !LeftZ,
            isXOverlap && isYOverlap && LeftZ && !RightZ
        };

        if (isXOverlap) { 
            if (!hasJustXoverlapped)
            {
                hasJustXoverlapped = true;
                xOverlapStartTime = Time.time;
            }
        } else
        {
            hasJustXoverlapped = false;
        }
    }

    bool hasJustXoverlapped = false;
    float xOverlapStartTime = 0;

    void UpdateVisuals()
    {
        for (int i = 0; i < states.Length; i++)
        {
            stateVisualMarkers[i].SetActive(states[i]);
            images[i].SetActive(states[i]);
        }
        images[6].SetActive(isXOverlap && isYOverlap && isZOverlap);


        particleEffects.SetActive((isXOverlap && isYOverlap && isZOverlap));
        
        XPlane.SetActive(true);
        YPlane.SetActive(isXOverlap && Time.time > xOverlapStartTime + 3);
        if (!YPlane.activeSelf)
        {
            YUp.SetActive(false);
            YDown.SetActive(false);
        }
        ZPlane.SetActive(isXOverlap && isYOverlap);

        xPlaneBarMesh.material = isXOverlap ? greenMaterial : redMaterial;
        yPlaneBarMesh.material = isYOverlap ? greenMaterial : redMaterial;
        zPlaneBarMesh.material = isZOverlap ? greenMaterial : redMaterial;

        XLeft.transform.localScale = new Vector3(
            diffXLeft,
            XLeft.transform.localScale.y,
            XLeft.transform.localScale.z);
        XLeft.transform.localPosition = new Vector3(
            trackedObjectA.transform.localPosition.x + offsetXLeft,
            XLeft.transform.localPosition.y,
            XLeft.transform.localPosition.z
            );
        XRight.transform.localScale = new Vector3(
            diffXRight,
            XRight.transform.localScale.y,
            XRight.transform.localScale.z);
        XRight.transform.localPosition = new Vector3(
            trackedObjectA.transform.localPosition.x + offsetXRight,
            XRight.transform.localPosition.y,
            XRight.transform.localPosition.z
            );

        YUp.transform.localScale = new Vector3(
            YUp.transform.localScale.x,
            diffYUp,
            YUp.transform.localScale.z);
        YUp.transform.localPosition = new Vector3(
            YUp.transform.localPosition.x,
            trackedObjectA.transform.localPosition.y + offsetYUp,
            YUp.transform.localPosition.z
            );
        YDown.transform.localScale = new Vector3(
            YDown.transform.localScale.x,
            diffYDown,
            YDown.transform.localScale.z);
        YDown.transform.localPosition = new Vector3(
            YDown.transform.localPosition.x,
            trackedObjectA.transform.localPosition.y + offsetYDown,
            YDown.transform.localPosition.z
            );

        ZLeft.transform.localScale = new Vector3(
            ZLeft.transform.localScale.x,
            ZLeft.transform.localScale.y,
            diffZLeft);
        ZLeft.transform.localPosition = new Vector3(
            ZLeft.transform.localPosition.x,
            ZLeft.transform.localPosition.y,
            trackedObjectA.transform.localPosition.z + offsetZLeft
            );
        ZRight.transform.localScale = new Vector3(
            ZLeft.transform.localScale.x,
            ZRight.transform.localScale.y,
            diffZRight);
        ZRight.transform.localPosition = new Vector3(
            ZRight.transform.localPosition.x,
            ZRight.transform.localPosition.y,
            trackedObjectA.transform.localPosition.z + offsetZRight
            );

        xNotchAValue.SetText(Math.Round(-1 * xProjection.axisEdgeNotchA.transform.localPosition.z, 2).ToString());
        xNotchBValue.SetText(Math.Round(-1 * xProjection.axisEdgeNotchB.transform.localPosition.z, 2).ToString());
        yNotchAValue.SetText(Math.Round(-1 * yProjection.axisEdgeNotchA.transform.localPosition.z, 2).ToString());
        yNotchBValue.SetText(Math.Round(-1 * yProjection.axisEdgeNotchB.transform.localPosition.z, 2).ToString());
        zNotchAValue.SetText(Math.Round(-1 * zProjection.axisEdgeNotchA.transform.localPosition.z, 2).ToString());
        zNotchBValue.SetText(Math.Round(-1 * zProjection.axisEdgeNotchB.transform.localPosition.z, 2).ToString());
    }
}
