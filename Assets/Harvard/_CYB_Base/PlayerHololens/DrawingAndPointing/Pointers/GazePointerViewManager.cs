using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazePointerViewManager : MonoBehaviour
{

    public PenTipModel penModel;
    public GazePointerModel pointerModel;

    public GameObject view1;
    //public GameObject view2;
    public GameObject view3;

    // Start is called before the first frame update
    void Start()
    {
        penModel.modelChangedLastTriggerEntered += PenModelEvent_ViewUpdated;
        pointerModel.onViewUpdate += PointerModelEvent_ViewUpdated;
    }

    public void PointerModelEvent_ViewUpdated()
    {
        view1.SetActive(pointerModel.ViewType == 1);
        //view2.SetActive(pointerModel.ViewType == 2);
        view3.SetActive(pointerModel.ViewType == 3);
    }


    public void PenModelEvent_ViewUpdated(string newMode)
    {
        if (newMode == "Pointer1")
        {
            pointerModel.ViewType = 1;
        }
        else if (newMode == "Pointer3")
        {
            pointerModel.ViewType = 3;
        }
        else
        {
            pointerModel.ViewType = 0;
        }
    }




    // Update is called once per frame
    void Update()
    {
        
    }
}
