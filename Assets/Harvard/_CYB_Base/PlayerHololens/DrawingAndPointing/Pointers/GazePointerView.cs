using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazePointerView : MonoBehaviour
{
    public GazePointerModel pointerModel;
    public Material lineMaterial;

    LineRenderer lineRenderer;

    void Start()
    {
        pointerModel.onPositionUpdate += ChangePos;
        pointerModel.onDirectionUpdate += ChangeDirection;

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = .005f;
        lineRenderer.endWidth = .005f;
        lineRenderer.positionCount = 2;
        lineRenderer.material = lineMaterial;


    }

    public void ChangePos()
    {
        lineRenderer.SetPosition(0, pointerModel.StartPos);

    }

    public void ChangeDirection()
    {
        transform.rotation = pointerModel.Direction;
        lineRenderer.SetPosition(1, transform.forward * 5 + transform.position);

    }

    private void OnDestroy()
    {
        pointerModel.onPositionUpdate -= ChangePos;
        pointerModel.onPositionUpdate -= ChangeDirection;

    }
}
