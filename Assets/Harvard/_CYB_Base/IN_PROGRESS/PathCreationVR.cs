using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCreationVR : PenScript
{

    public GameObject pathPrefab;

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentMode == Mode.FirstDown)
        {
            // still dragging end
            currentPathSegment.endPos = penTipManager.penTip.transform.position;
            //Debug.Log(currentPathSegment.endPos);
        }



        // UI EVENTS

        if (penTipManager.debug_Pen.simulatedButton1.NowClicked)
        //if (Input.GetKeyDown("c"))
        {
            if (currentMode == Mode.Idle)
            {
                // drop first point
                GameObject pathObj = GameObject.Instantiate(pathPrefab);
                currentPathSegment = pathObj.GetComponent<PathSegment>();
                currentPathSegment.startPos = penTipManager.penTip.transform.position;
                currentPathSegment.endPos = penTipManager.penTip.transform.position;

                currentMode = Mode.FirstDown;

            }
            else if (currentMode == Mode.FirstDown)
            {
                // just dropped second point
                currentPathSegment.endPos = penTipManager.penTip.transform.position;
                currentMode = Mode.Idle;
            }
        }
    }


    public enum Mode
    {
        Idle,
        FirstDown
    }

    public Mode currentMode;

    public PathSegment currentPathSegment;

    private void OnGUI()
    {
        
    }
}
