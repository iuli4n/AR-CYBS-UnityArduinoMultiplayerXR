using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugUI_DrawingPlanes : MonoBehaviour
{


    public List<GameObject> drawingPlanes = new List<GameObject>();
    private int currentDrawingPlaneIndex = 0;
    private GameObject currentDrawingPlane = null;


    // Start is called before the first frame update
    void Start()
    {
        //DebugUI_MenuAndKeys.Instance.onEvent_DrawPlane_Next = Update_DrawingPlane();
        Update_DrawingPlane();
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void Update_DrawingPlane()
    {
        currentDrawingPlane = drawingPlanes[currentDrawingPlaneIndex];
    }
    public void OnEvent_DrawingPlaneNext()
    {
        currentDrawingPlane.SetActive(false);
        currentDrawingPlaneIndex = (currentDrawingPlaneIndex + 1) % drawingPlanes.Count;
        Update_DrawingPlane();
        currentDrawingPlane.SetActive(true);
    }
    public void OnEvent_DrawingPlaneHide()
    {
        currentDrawingPlane.SetActive(false);
    }
}
