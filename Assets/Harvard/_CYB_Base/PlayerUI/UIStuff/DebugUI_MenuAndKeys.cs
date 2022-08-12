using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DebugUI_MenuAndKeys : MonoBehaviour
{
    public static DebugUI_MenuAndKeys Instance;

    public KeyCode DrawPlane_Next_key; public UnityEvent DrawPlane_Next_action;
    public KeyCode DrawPlane_Hide_key; public UnityEvent DrawPlane_Hide_action;
    public KeyCode UndoRedo_Undo_key; public UnityEvent UndoRedo_Undo_action;
    public KeyCode UndoRedo_Redo_key; public UnityEvent UndoRedo_Redo_action;

    public KeyCode DrawTool_Select_key; public UnityEvent DrawTool_Select_action;

    public string[] otherKeys_description;
    public KeyCode[] otherKeys; public UnityEvent[] otherEvents;

    /*
    public KeyCode PointerTool_Select0_key; public UnityEvent PointerTool_Select0_action;
    public KeyCode PointerTool_Select1_key; public UnityEvent PointerTool_Select1_action;
    public KeyCode PointerTool_Select2_key; public UnityEvent PointerTool_Select2_action;
    */

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(Instance == null, "Cannot have multiple versions of this script in the scene !");
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(DrawPlane_Next_key))
            DrawPlane_Next_action?.Invoke();
        if (Input.GetKeyDown(DrawPlane_Hide_key))
            DrawPlane_Hide_action?.Invoke();
        if (Input.GetKeyDown(UndoRedo_Undo_key))
            UndoRedo_Undo_action?.Invoke();
        if (Input.GetKeyDown(UndoRedo_Redo_key))
            UndoRedo_Redo_action?.Invoke();
        if (Input.GetKeyDown(DrawTool_Select_key))
            DrawTool_Select_action?.Invoke();

        for (int i=0; i<otherKeys.Length; i++)
        {
            if (Input.GetKeyDown(otherKeys[i]))
            {
                otherEvents[i]?.Invoke();
            }
        }
    }

    void OnGUI()
    {
        // CURSOR STUFF

        /*
        //Press this button to lock the Cursor
        if (GUI.Button(new Rect(125, 0, 100, 50), "Lock Cursor"))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        */

        //Press this button to confine the Cursor within the screen
        /*
        if (GUI.Button(new Rect(0, 0, 100, 50), "Confine Cursor"))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        */
    
        // LABELS STUFF
        
        /*
        GUILayout.BeginHorizontal();

        GUILayout.Label("");
        //GUILayout.Label("ANCHOR: " + DesktopUI_MouseRender.Instance.DrawingAnchor_GetCurrent());

        GUILayout.EndHorizontal();
        */
    }
}
