//using Harvard.ModelNetworking.Extras;
using Microsoft.MixedReality.Toolkit.Input;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
//using System.Drawing;
using UnityEngine;



/****
 * NEXT STEPS jul 2021
 * - Before switching into a new mode make sure to call StopPreviousMode() instead of manually disabling things
 * - Remove RPC_* because we're not using rpcs anymore
 * - Change PenTipModel to AtomicDataModelString
 ***/

public class ActivateTip : MonoBehaviour//PunCallbacks
{
    public string currentMode = "";

    // TODO: Merge Debug_Pen into this ActivateTip class
    public Debug_Pen debug_Pen;

    public GameObject drawingParentPrefab;

    public GameObject penTip;
    public GameObject targetQuad;
    //public Color colorPicked;

    public GameObject newDrawingParent;
    public GameObject sliderColorPicker;
    public GameObject floatingMenu;

    public DebugUI_UndoRedo undoRedoManager;


    private Vector3 drawingParentPos;


    // True if this pentip is for the person who's using this device (as opposed to pen tips attached to networked players)
    public bool BelongsToLocalPlayer { get { return LocalPlayerUIManager.Instance.actualPointerTip == this.gameObject;  } }


    // :MODELNETWORKINGOUT: // private ObjectPositioningControlScript objectPositioningScript;

    // Use these to find out if the mouse is clicked on the computer that's controlling this object
    /***
    public bool GetInputMouseButton1 { get { return objectPositioningScript.mouseButton1.Clicked; } }
    public bool GetInputMouseButton1Down { get { return objectPositioningScript.mouseButton1.NowClicked; } }
    public bool GetInputMouseButton1Up { get { return objectPositioningScript.mouseButton1.NowUnclicked; } }

    public bool IsControlledLocally { get { return objectPositioningScript.iAmControllingTheTransform; } }

    ****/

    // Stores the current state of the pen tip
    public PenTipModel model;


    // Start is called before the first frame update
    void Start()
    {
        targetQuad.GetComponent<MeshRenderer>().enabled = false;
        //colorPicked = Color.red;
        drawingParentPos = new Vector3(0f, 0f, 0f);


        model = gameObject.GetComponent<PenTipModel>();
        model.modelChangedLastTriggerEntered += ModelEvent_OnChangedLastTriggerEntered;


        // :MODELNETWORKINGOUT: // objectPositioningScript = gameObject.GetComponent<ObjectPositioningControlScript>();
        // :MODELNETWORKINGOUT: // DebugUtilities.Assert(objectPositioningScript != null, "ERROR: ActivateTip needs an object positioning script to function");
    }


    void DeactivateAllModes()
    {
        penTip.GetComponent<DropApin>().enabled = false;
        penTip.GetComponent<DrawingVR>().enabled = false;
        penTip.GetComponent<DropMic>().enabled = false;
        penTip.GetComponent<Eraser>().enabled = false;
        penTip.GetComponent<PathCreationVR>().enabled = false;

        sliderColorPicker.SetActive(false);
    }

    public void Local_ActivatePointer1()
    {
        model.lastTriggerEntered = "Pointer1";
    }
    public void Local_ActivatePointer3()
    {
        model.lastTriggerEntered = "Pointer3";
    }
    public void Local_ActivatePointer4Path()
    {
        model.lastTriggerEntered = "PointerPath";
    }
    public void Local_ActivatePointerRESET()
    {
        model.lastTriggerEntered = "";
    }

    public void Local_SetDrawingColor(int i)
    {
        penTip.GetComponent<DrawingVR>().penColorModel.Value = i;
    }

    public void Local_ActivateDrawing()
    {
        model.lastTriggerEntered = "DrawBtn";
    }

    public void DoActivateDrawing()
    {
        RPC_ActuallyDoActivateDrawing();
        //FAKEDPHOTON PhotonView.Get(this).RPC("RPC_ActuallyDoActivateDrawing", RpcTarget.All);
    }

    //[PunRPC]
    public void RPC_ActuallyDoActivateDrawing()
    {
        penTip.GetComponent<DropApin>().enabled = false;
        penTip.GetComponent<DropMic>().enabled = false;

        penTip.GetComponent<DrawingVR>().penTipManager = this;
        penTip.GetComponent<DrawingVR>().enabled = true;
        sliderColorPicker.SetActive(true);

        //penTip.GetComponent<Eraser>().enabled = false;

        // TODO: This should all be in an initialize function inside the drawVR script

        GameObject newObject = GameObject.Instantiate(drawingParentPrefab, drawingParentPos, Quaternion.identity);
        RPC_InitializeDrawingParent_noPhoton(newObject);
        /**
        GameObject newObject = PhotonNetwork.Instantiate(drawingParentPrefab.name, drawingParentPos, Quaternion.identity);
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("RPC_InitializeDrawingParent", RpcTarget.All,
            newObject.GetComponent<PhotonView>().ViewID);
        **/

        currentMode = model.lastTriggerEntered;
    }

    [PunRPC]
    public void RPC_InitializeDrawingParent(int newObjectID)
    {
        newDrawingParent = PhotonNetwork.GetPhotonView(newObjectID).gameObject;
        newDrawingParent.transform.parent = this.transform.parent;
        newDrawingParent.GetComponent<MeshRenderer>().enabled = false;
    }
    public void RPC_InitializeDrawingParent_noPhoton(GameObject drawingParent)
    {
        newDrawingParent = drawingParent;
        newDrawingParent.transform.parent = this.transform.parent;
        newDrawingParent.GetComponent<MeshRenderer>().enabled = false;
    }

    /* TODO: RPC */
    public void ActivateEraser()
    {
        model.lastTriggerEntered = "EraserBtn";
    }
    public void DoActivateEraser()
    {
        penTip.GetComponent<DropApin>().enabled = false;
        penTip.GetComponent<DrawingVR>().enabled = false;
        penTip.GetComponent<DropMic>().enabled = false;
        //penTip.GetComponent<Eraser>().enabled = true;
        sliderColorPicker.SetActive(false);


        Destroy(GameObject.FindGameObjectWithTag("DrawParentTag"));

        //newDrawingParent.GetComponent<MeshRenderer>().enabled = true;

        currentMode = model.lastTriggerEntered;
    }

    public void ActivatePin()
    {
        model.lastTriggerEntered = "PinBtn";
    }
    public void DoActivatePin()
    {
        RPC_ActuallyDoActivatePin();
        //FAKEPHOT PhotonView.Get(this).RPC("RPC_ActuallyDoActivatePin", RpcTarget.All);
    }

    [PunRPC]
    public void RPC_ActuallyDoActivatePin()
    {
        penTip.GetComponent<DropApin>().enabled = true;
        penTip.GetComponent<DropMic>().enabled = false;
        penTip.GetComponent<DrawingVR>().enabled = false;
        //penTip.GetComponent<Eraser>().enabled = false;
        sliderColorPicker.SetActive(false);

        currentMode = model.lastTriggerEntered;
    }

    public void ActivateMic()
    {
        model.lastTriggerEntered = "MicBtn";
    }
    public void DoActivateMic()
    {
        penTip.GetComponent<DropApin>().enabled = false;
        penTip.GetComponent<DrawingVR>().enabled = false;
        //penTip.GetComponent<Eraser>().enabled = false;
        penTip.GetComponent<DropMic>().enabled = true;
        penTip.GetComponent<DropMic>().micDetected = false;
        sliderColorPicker.SetActive(false);

        currentMode = model.lastTriggerEntered;
    }

    public void DoActivatePath()
    {
        penTip.GetComponent<PathCreationVR>().enabled = true;

        currentMode = model.lastTriggerEntered;
    }

    public void ActivateMenu()
    {
        floatingMenu.SetActive(true);
    }


    // called whenever the model changed (locally or in another player), and when the last value is different than the new value
    private void ModelEvent_OnChangedLastTriggerEntered(string newTriggerName)
    {
        OnModelPenChanged(newTriggerName);
    }

    private void OnModelPenChanged(string menuItemName)
    {
        if (false /*FAKEPHOT !photonView.IsMine*/)
        {
            // ignore pen tips that aren't mine
            return;
        }

        if (menuItemName == currentMode)
        {
            // don't do anything because we're already in this mode
            return;
        }

        // turn off previous mode
        DeactivateAllModes();

        // TODO:PRETTY: turn this into a switch/case

        if (menuItemName == "")
        {
            // we are going back to default mode. hopefully someone else knows about this ;)

            //EditingManager.Instance.SwitchPointerTo(TEMP_Debug_Pointers.PointerType.Default);
            /** kept here just in case we need it later
            // idle
            EditingManager.Instance.editingAllowed = true;
            // TODO: move these inside EditingManager
            PointerUtils.SetHandPokePointerBehavior(PointerBehavior.Default);
            PointerUtils.SetHandRayPointerBehavior(PointerBehavior.Default);
            **/
        }

        if (menuItemName == "DrawBtn")
        {
            EditingManager.Instance.SwitchPointerTo(TEMP_Debug_Pointers.PointerType.OriginalTool_Drawing);
            
            DoActivateDrawing();
            
            /** kept here just in case we need it later
            
            EditingManager.Instance.editingAllowed = false;
            // Turn off all hand rays
            PointerUtils.SetHandPokePointerBehavior(PointerBehavior.AlwaysOff);
            PointerUtils.SetHandRayPointerBehavior(PointerBehavior.AlwaysOff);
            */
        }

        if (menuItemName == "PinBtn")
        {
            DoActivatePin();
        }

        if (menuItemName == "MicBtn")
        {
            Debug.Assert(false, "NOT IMPLEMENTED - CODE NEEDS UPDATE HERE");
            DoActivateMic();
        }

        if (menuItemName == "EraserBtn")
        {
            Debug.Assert(false, "NOT IMPLEMENTED - CODE NEEDS UPDATE HERE"); 
            DoActivateEraser();
        }

        if (menuItemName == "PointerPath")
        {
            Debug.Assert(false, "NOT IMPLEMENTED - CODE NEEDS UPDATE HERE"); 
            DoActivatePath();
        }

        if (menuItemName == "Pointer1" || menuItemName == "Pointer3")
        {
            Debug.Assert(false, "NOT IMPLEMENTED - CODE NEEDS UPDATE HERE");

            // but record this is the present mode
            //currentMode = model.lastTriggerEntered;
        }

        // CHECK:I hope no bugs introduced here
        currentMode = model.lastTriggerEntered;
    }

}
