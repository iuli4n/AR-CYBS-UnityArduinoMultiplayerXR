using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectionManager : MonoBehaviour
{
    public GameObject trackedObjectA;
    public GameObject projectedPointA;

    public GameObject axisEdgeNotchA;
    public GameObject axisThresholdRectA;

    public GameObject trackedObjectB;
    
    public GameObject axisEdgeNotchB;

    public float barOffset;
    public float notchOffset;

    void Start()
    {
        
    }

    void Update()
    {
        // track object A. Project it onto the plane, and update the axis edge notches/thresholds
        projectedPointA.transform.localPosition = ProjectToMe(trackedObjectA.transform.position);
        axisEdgeNotchA.transform.localPosition = AlignToZEdge(projectedPointA.transform.localPosition);
        axisEdgeNotchA.transform.localPosition = new Vector3(axisEdgeNotchA.transform.localPosition.x + notchOffset, axisEdgeNotchA.transform.localPosition.y, axisEdgeNotchA.transform.localPosition.z);
        axisThresholdRectA.transform.localPosition = AlignToZEdge(projectedPointA.transform.localPosition);
        axisThresholdRectA.transform.localPosition = new Vector3(axisThresholdRectA.transform.localPosition.x, axisThresholdRectA.transform.localPosition.y, axisThresholdRectA.transform.localPosition.z + barOffset);

        // track object B
        axisEdgeNotchB.transform.localPosition = AlignToZEdge(ProjectToMe(trackedObjectB.transform.position));
        axisEdgeNotchB.transform.localPosition = new Vector3(axisEdgeNotchB.transform.localPosition.x + notchOffset, axisEdgeNotchB.transform.localPosition.y, axisEdgeNotchB.transform.localPosition.z);
    }



    Vector3 ProjectToMe(Vector3 trackedWorldPos)
    {
        // project a world space point onto my plane
        // returns local position of the projected point

        Vector3 worldPos =
            Vector3.ProjectOnPlane(trackedWorldPos, transform.up) +
            Vector3.Dot(transform.position, transform.up) * transform.up;

        Vector3 localPos = this.transform.InverseTransformPoint(worldPos);

        return localPos;

    }

    Vector3 AlignToZEdge(Vector3 myObjectLocalPos, float xoffset = 5f, float yoffset = 0f)
    {
        // returns local position of myObject when its local position x,y have been zeroed (and shifted by specified offsets)

        return new Vector3(xoffset, yoffset, myObjectLocalPos.z);
    }
}
