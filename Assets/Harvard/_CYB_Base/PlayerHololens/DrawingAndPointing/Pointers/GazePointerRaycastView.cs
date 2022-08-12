using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazePointerRaycastView : MonoBehaviour
{
    public GazePointerModel pointerModel;
    public Material lineMaterial;
    //public GameObject indicator;

    LineRenderer lineRenderer;
    public GameObject indicatorInstance;

    void Start()
    {
        pointerModel.onPositionUpdate += ChangeRaycast;
        pointerModel.onDirectionUpdate += ChangeDirection;

        //indicatorInstance = Instantiate(indicator, pointerModel.StartPos, Quaternion.identity);
        indicatorInstance.name = "PointerIndicator";


        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = .005f;
        lineRenderer.endWidth = .005f;
        lineRenderer.positionCount = 2;
        lineRenderer.material = lineMaterial;
       

    }

    public void ChangeRaycast()
    {

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(pointerModel.StartPos, transform.TransformDirection(Vector3.forward), out hit, 5))
        {

            lineRenderer.SetPosition(0, pointerModel.StartPos);
            lineRenderer.SetPosition(1, hit.point);

            indicatorInstance.transform.position = hit.point;
            indicatorInstance.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);


            Debug.DrawRay(pointerModel.StartPos, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            //Debug.Log("Did Hit");
        }
        else
        {
            lineRenderer.SetPosition(0, pointerModel.StartPos);
            lineRenderer.SetPosition(1, transform.TransformDirection(Vector3.forward) * 5 + transform.position);

            indicatorInstance.transform.position = new Vector3 (50, 50, 50);

            /* 
            this works but the indicator is offset so i didn't use it for now
            
            indicatorInstance.transform.position = pointerModel.StartPos + transform.forward * 5;
            indicatorInstance.transform.rotation = Quaternion.FromToRotation(Vector3.up, transform.TransformDirection(Vector3.forward)); 
            */


            Debug.DrawRay(pointerModel.StartPos, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            //Debug.Log("Did not Hit");
        }

    }

    public void ChangeDirection()
    {
        transform.position = pointerModel.StartPos;
        transform.rotation = pointerModel.Direction;
    }

    private void OnDestroy()
    {
        pointerModel.onPositionUpdate -= ChangeRaycast;
        pointerModel.onPositionUpdate -= ChangeDirection;

    }
}
