using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditingManager : MonoBehaviour
{
    // DESCRIPTION: This is in charge of making sure each object can/can't be interacted by the current pointer.
    // This actually switches pointers and talks to all the objects' PointerInteractionConfigurer so they can be interacted appropriately
    // by the current pointer.

    public static EditingManager Instance;

    public bool showGUI = false;
    public bool guiMode_simplified = true;
    public TEMP_Debug_Pointers pointersManager;

    private GameObject lastEdited;

    [SerializeField]
    private List<PointerInteractionConfigurer> managedNormalInteractiveObjects = new List<PointerInteractionConfigurer>();
    [SerializeField]
    private List<PointerInteractionConfigurer> managedSpecialInteractiveObjects = new List<PointerInteractionConfigurer>();


    public EffectsMenu effectsButton;
    public Debug_RPCEnabler TEMP_DEBUG_enabledisableSpecialCreation;

    public bool startInCreationMode = true;

    // Invoked whenever the last edited object changes (ie: when the user grabs a new object).
    public event Action<GameObject> onEditedObjectChanged;

    // Start is called before the first frame update
    void Awake()
    {
        Debug.Assert(!Instance, "Only supposed to have one of these objects !");
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(CheckStartMode());
    }
    IEnumerator CheckStartMode()
    {
        if (startInCreationMode)
        {
            // wait till the network initializes
            while (PlayersManager.Instance == null || PlayersManager.Instance.localPlayerFingerPenTip == null)
            {
                yield return new WaitForSeconds(0.2f);
            }
            yield return new WaitForSeconds(1f);
            SwitchPointerTo(TEMP_Debug_Pointers.PointerType.Create_Default);
        }
    }

    public void BeginEditing(GameObject o)
    {
        if (lastEdited != o)
        {
            lastEdited = o;
            onEditedObjectChanged?.Invoke(o);
        }
        
    }
    public void EndEditing(GameObject o)
    {
    }

    public GameObject GetLastEdited()
    {
        return lastEdited;
    }
    public bool CanMoveThis(GameObject o)
    {
        var pic = o.GetComponent<PointerInteractionConfigurer>();
        if (pic==null)
        {
            // object doesn't contain PointerInteractionZConfigurer, so let the caller figure it out
            return true;
        } else
        {
            return pic.nowallowMoving;
        }
    }
    public bool CanScaleThis(GameObject o)
    {
        var pic = o.GetComponent<PointerInteractionConfigurer>();
        if (pic == null)
        {
            // object doesn't contain PointerInteractionZConfigurer, so let the caller figure it out
            return true;
        }
        else
        {
            return pic.nowallowScaling;
        }
    }

    public void ConfigureObject(PointerInteractionConfigurer pic, 
        bool allowMove, bool allowScaling, bool allowScalingShowHandles, bool allowSpecialMove)
    {
        if (!pic.isSpecialObject)
        {
            pic.ConfigureInteraction(allowMove, allowScaling, allowScalingShowHandles, allowMove || allowScaling, true);
        } else
        {
            pic.ConfigureInteraction(allowSpecialMove, false, false, true, false);
        }
    }

    public void RegisterPlayerObject(PlayerCreatedObject o)
    {
        bool allowMoving, allowScaling, allowScalingShowHandles, allowSpecialMoving;
        GetPermissionsForCurrentPointer(out allowMoving, out allowScaling, out allowScalingShowHandles, out allowSpecialMoving);

        foreach (var pic in o.gameObject.GetComponentsInChildren<PointerInteractionConfigurer>(true))
        {
            if (!pic.isSpecialObject)
            {
                if (!managedNormalInteractiveObjects.Contains(pic))
                    managedNormalInteractiveObjects.Add(pic);
            }
            else
            {
                // it's a special object, like path endpoint
                if (!managedSpecialInteractiveObjects.Contains(pic))
                    managedSpecialInteractiveObjects.Add(pic);
            }

            ConfigureObject(pic, allowMoving, allowScaling, allowScalingShowHandles, allowSpecialMoving);
        }
    }
    public void UnRegisterPlayerObject(PlayerCreatedObject o)
    {
        foreach (var pic in o.gameObject.GetComponentsInChildren<PointerInteractionConfigurer>(true))
        {
            if (!pic.isSpecialObject)
                managedNormalInteractiveObjects.Remove(pic);
            else
                managedSpecialInteractiveObjects.Remove(pic);
        }
    }



    private void SwitchOldPointerTo(TEMP_Debug_Pointers.PointerType p)
    {
        if (p == TEMP_Debug_Pointers.PointerType.OriginalTool_Drawing)
        {
            PlayersManager.Instance.localPlayerFingerPenTip.Local_ActivateDrawing();
        }
        else if (p == TEMP_Debug_Pointers.PointerType.OriginalTool_Point1)
        {
            PlayersManager.Instance.localPlayerFingerPenTip.Local_ActivatePointer1();
        }
        else if (p == TEMP_Debug_Pointers.PointerType.OriginalTool_Point2)
        {
            PlayersManager.Instance.localPlayerFingerPenTip.Local_ActivatePointer3();
        } else
        {
            PlayersManager.Instance.localPlayerFingerPenTip.Local_ActivatePointerRESET();
        }
    }
    private void GetPermissionsForCurrentPointer(out bool allowNormalMoving, out bool allowNormalScaling, out bool allowNormalScalingShowHandles, out bool allowSpecialMoving)
    {
        // by default nothing is enabled
        allowSpecialMoving = false;
        allowNormalMoving = false;
        allowNormalScaling = false;
        allowNormalScalingShowHandles = false;


        var pointerType = TEMP_Debug_Pointers.Instance.currentPointerType;

        if (pointerType == TEMP_Debug_Pointers.PointerType.OriginalTool_Drawing)
        {
            // no interaction is enabled
        }
        else if (pointerType == TEMP_Debug_Pointers.PointerType.Edit_PrecisionMove)
        {
            allowNormalMoving = true;
        }
        else if (pointerType == TEMP_Debug_Pointers.PointerType.Edit_SpecialPrecisionMove)
        {
            allowSpecialMoving = true;
        }
        else if (pointerType == TEMP_Debug_Pointers.PointerType.Edit_PrecisionScale)
        {
            allowNormalScaling = true;
            allowNormalScalingShowHandles = true;
        }
        else if (pointerType == TEMP_Debug_Pointers.PointerType.Create_PrecisionCreate)
        {
            // no interaction is enabled
        }
        else if (pointerType == TEMP_Debug_Pointers.PointerType.Create_Default)
        {
            allowNormalMoving = true;
        }
        else if (pointerType == TEMP_Debug_Pointers.PointerType.NoControl)
        {
            // no interaction is enabled
        }
        else
        {
            Debug.LogError("Can't figure pointer because I don't know what mode this is: " + pointerType);
        }

    }

    // Change the pointer to the one specified in the string (string corresponds to enums of PointerType)
    public void SwitchPointerToByS(string pointerTypeString)
    {
        SwitchPointerTo((TEMP_Debug_Pointers.PointerType)Enum.Parse(typeof(TEMP_Debug_Pointers.PointerType), pointerTypeString));
    }

    // Change the pointer to the one specified 
    public void SwitchPointerTo(TEMP_Debug_Pointers.PointerType p)
    {
        if (TEMP_Debug_Pointers.Instance.currentPointerType == p)
        {
            // nothing to do
            return;
        }

        // For the legacy fingertip pointers (ex: drawings, lasers), make sure those match what's currently selected.
        // TODO: Clean this. 
        SwitchOldPointerTo(p);

        // Then switch the pointer (note, this triggers changes through TEMP_Debug_Pointers)
        pointersManager.SwitchPointerTo(p);


        // Figure out the object interaction permissions of this pointer
        bool allowMoving, allowScaling, allowScalingShowHandles, allowSpecialMoving;
        GetPermissionsForCurrentPointer(out allowMoving, out allowScaling, out allowScalingShowHandles, out allowSpecialMoving);

        // Then update interaction configurations for everyone in our scene
        foreach (var pic in managedNormalInteractiveObjects)
        {
            ConfigureObject(pic, allowMoving, allowScaling, allowScalingShowHandles, allowSpecialMoving);
        }
        foreach (var pic in managedSpecialInteractiveObjects)
        {
            ConfigureObject(pic, allowMoving, allowScaling, allowScalingShowHandles, allowSpecialMoving);
        }
    }




    // Update is called once per frame
    void Update()
    {
        
    }

    private bool GUIPointerButton(string s, TEMP_Debug_Pointers.PointerType p)
    {
        if (TEMP_Debug_Pointers.Instance.currentPointerType == p)
            s = "[" + s + "]";

        if (GUILayout.Button(s))
        {
            SwitchPointerTo(p);
            return true;
        }

        return false;
    }

    private void OnGUI()
    {
        if (!showGUI)
            return;



        GUILayout.Label("             ");
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace(); 
        GUIPointerButton("P", TEMP_Debug_Pointers.PointerType.NoControl);
        GUIPointerButton("C", TEMP_Debug_Pointers.PointerType.Create_Default);
        GUIPointerButton("Es", TEMP_Debug_Pointers.PointerType.Edit_PrecisionScale);

        if (!guiMode_simplified)
        {
            // show other complicated options

            GUIPointerButton("Em", TEMP_Debug_Pointers.PointerType.Edit_PrecisionMove);
            GUIPointerButton("SEm", TEMP_Debug_Pointers.PointerType.Edit_SpecialPrecisionMove);

            /**
            if (GUIPointerButton("Ka", TEMP_Debug_Pointers.PointerType.Create_PrecisionCreate))
            {
                TEMP_DEBUG_enabledisableSpecialCreation.EnableObject(1);
            }
            if (GUIPointerButton("Kt", TEMP_Debug_Pointers.PointerType.Create_PrecisionCreate))
            {
                TEMP_DEBUG_enabledisableSpecialCreation.EnableObject(0);
            }
            **/
            if (GUILayout.Button("Kt"))
            {
                TEMP_Debug_Pointers.Instance.SwitchPointerTo(TEMP_Debug_Pointers.PointerType.Create_PrecisionCreate);
                CreationTipManager.Instance.StartCreating_Tooltip();
            }
            if (GUILayout.Button("Ka"))
            {
                TEMP_Debug_Pointers.Instance.SwitchPointerTo(TEMP_Debug_Pointers.PointerType.Create_PrecisionCreate);
                CreationTipManager.Instance.StartCreating_Arrow();
            }
            if (GUILayout.Button("Kp"))
            {
                TEMP_Debug_Pointers.Instance.SwitchPointerTo(TEMP_Debug_Pointers.PointerType.Create_PrecisionCreate);
                CreationTipManager.Instance.StartCreating_Path();
            }
        }
        
        if (GUILayout.Button("D"))
        {
            SwitchPointerToByS("OriginalTool_Drawing");
        }
        if (GUILayout.Button("P1"))
        {
            SwitchPointerToByS("OriginalTool_Point1");
        }
        if (GUILayout.Button("P2"))
        {
            SwitchPointerToByS("OriginalTool_Point2");
        }


        
        if (!guiMode_simplified)
        {
            // show other complicated optionsGUILayout.BeginHorizontal();
            if (GUILayout.Button("FX"))
            {
                effectsButton.ToggleEffectsMenu();
            }
            //GUILayout.FlexibleSpace();
            //GUILayout.EndHorizontal();
        }

        //GUIPointerButton("Cp", TEMP_Debug_Pointers.PointerType.Create_PrecisionCreate);
        GUILayout.EndHorizontal();
        GUILayout.Label("Current: " + TEMP_Debug_Pointers.Instance.currentPointerType);

    }
}
