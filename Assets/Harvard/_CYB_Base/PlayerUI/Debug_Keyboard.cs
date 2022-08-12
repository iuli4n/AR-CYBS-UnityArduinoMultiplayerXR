using System.Collections;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;

public class Debug_Keyboard : MonoBehaviour
{

    // TODO: Replace penTip with just the model for the pentip (and just change its value from here, everything else should follow)
    public ActivateTip penTip;
    public AtomicDataModelBool mouseButtonModel;
    public AtomicDataModelInt skinModel;


    // Start is called before the first frame update
    void Start()
    {
        PlayersManager.Instance.localPlayerFingerPenTip = penTip;
    }

    private const float PinchThreshold = 0.7f;

    public static bool IsPinching(Handedness trackedHand)
    {
        if (HandJointUtils.FindHand(trackedHand) == null) return false;
        return HandPoseUtils.CalculateIndexPinch(trackedHand) > PinchThreshold;
    }

    bool temp_lastskinpinch = false;

    void Update()
    {
        const Handedness hand = Handedness.Right;

        mouseButtonModel.Value = IsPinching(hand);//Input.GetKey(KeyCode.Z);


        if (Input.GetKeyDown("u"))
        {
            penTip.undoRedoManager.UndoRedo_Undo();
        }
        if (Input.GetKeyDown("i"))
        {
            penTip.undoRedoManager.UndoRedo_Redo();
        }
        /*
        if (Input.GetKeyDown("q"))
            //(!temp_lastskinpinch && IsPinching(Handedness.Left)) 
            //)
        {
            skinModel.Value = (skinModel.Value + 1) % 4;
        }
        */


        temp_lastskinpinch = mouseButtonModel.Value;// IsPinching(hand);

    }

    void OnGUI() { 

        GUILayout.BeginArea(new Rect(300, 0, 300, 600));

        GUILayout.Label("");
        GUILayout.Label("POINTER: "+ EditingManager.Instance.pointersManager.currentPointerType);
        GUILayout.Label("Current OT: " + PlayersManager.Instance.localPlayerFingerPenTip.currentMode);

        /***
        if (GUILayout.Button("RESET"))
        {
            penTip.Local_ActivatePointerRESET();
        }
        if (GUILayout.Button("DRAW"))
        {
            penTip.Local_ActivateDrawing();
        }
        
        if (GUILayout.Button("1"))
        {
            penTip.Local_ActivatePointer1();
        }
        if (GUILayout.Button("2"))
        {
            penTip.Local_ActivatePointer3();
        }
        if (GUILayout.Button("4Path"))
        {
            penTip.Local_ActivatePointer4Path();
        }
        ****/

        GameObject lastEditedObject = EditingManager.Instance.GetLastEdited();
        if (lastEditedObject)
        {
            GUILayout.Label("Last edited object: " + lastEditedObject.name);
            DrawingPropertiesManager dpm = lastEditedObject.GetComponent<DrawingPropertiesManager>();
            /**
            if (dpm)
            {
                GUILayout.Label("SetColor:");

                if (GUILayout.Button("0"))
                    dpm.SetColor(0);
                if (GUILayout.Button("1"))
                    dpm.SetColor(1);
                if (GUILayout.Button("2"))
                    dpm.SetColor(2);
                if (GUILayout.Button("3"))
                    dpm.SetColor(3);
                
                //if (GUILayout.Button("0"))
                //    penTip.SetDrawingColor(0);
                
            }
            **/
        
        } else
        {
            GUILayout.Label("No edits made");
        }
        GUILayout.EndArea();

    }
}
