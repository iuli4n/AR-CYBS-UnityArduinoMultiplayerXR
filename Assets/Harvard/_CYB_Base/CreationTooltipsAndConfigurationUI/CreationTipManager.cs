using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreationTipManager : MonoBehaviour
{
    public static CreationTipManager Instance;

    enum State
    {
        Idle,
        Positioning
    }
    State currentState;

    IPositioningTool currentActivePlacementTool;


    /*
    private Tool_Creator1Part_v2 tool1Part;
    private Tool_Creator2Part tool2Part;
    */

    public Tool_Creator2Part tool2Part_arrow;
    public Tool_Creator2Part tool2Part_tooltip;
    public Tool_Creator2Part tool2Part_path;

    //public Tool_Creator2Part[] DEBUG_CLICKOTHERTOOLS;

    private void OnEnable()
    {
        TEMP_Debug_Pointers.Instance.OnPointerSwitched += OnPointerSwitch;
    }
    private void OnDisable()
    {
        TEMP_Debug_Pointers.Instance.OnPointerSwitched -= OnPointerSwitch;
    }

    public void OnPointerSwitch(TEMP_Debug_Pointers.PointerType old, TEMP_Debug_Pointers.PointerType curr) 
    {
        if (old == TEMP_Debug_Pointers.PointerType.Create_PrecisionCreate && curr != TEMP_Debug_Pointers.PointerType.Create_PrecisionCreate)
        {
            if (currentActivePlacementTool != null)
            {
                OnToolDone(true);
            }
        }
    }

    public void StartCreating_Arrow()
    {
        if (currentActivePlacementTool != null)
        {
            currentActivePlacementTool.DoCancel();
            currentActivePlacementTool = null;
        }
        currentState = State.Positioning;

        //Debug.Log("Starting to create arrow");
        EditingManager.Instance.SwitchPointerTo(TEMP_Debug_Pointers.PointerType.Create_PrecisionCreate);

        //Debug.Log("Starting to create arrow 2");
        currentActivePlacementTool = tool2Part_arrow;
        //currentActivePlacementTool.SetPlacementCompleteCallback(OnToolDone);
        currentActivePlacementTool.gameObject.SetActive(true);
        //Debug.Log("Starting to create arrow 3");
        currentActivePlacementTool.DoStart(tool2Part_arrow.OVERRIDE_USETHISASPREFAB);
        //Debug.Log("Starting to create arrow 4");

    }
    public void StartCreating_Tooltip()
    {
        if (currentActivePlacementTool != null)
        {
            currentActivePlacementTool.DoCancel();
            currentActivePlacementTool = null;
        }
        currentState = State.Positioning;
        
        EditingManager.Instance.SwitchPointerTo(TEMP_Debug_Pointers.PointerType.Create_PrecisionCreate);

        currentActivePlacementTool = tool2Part_tooltip;
        //currentActivePlacementTool.SetPlacementCompleteCallback(OnToolDone);
        currentActivePlacementTool.gameObject.SetActive(true); 
        currentActivePlacementTool.DoStart(tool2Part_tooltip.OVERRIDE_USETHISASPREFAB);
    }

    /// TEMPORARY
    public void StartCreating_Path()
    {
        if (currentActivePlacementTool != null)
        {
            currentActivePlacementTool.DoCancel();
            currentActivePlacementTool = null;
        }
        currentState = State.Positioning;

        //Debug.Log("Starting to create arrow");
        EditingManager.Instance.SwitchPointerTo(TEMP_Debug_Pointers.PointerType.Create_PrecisionCreate);

        //Debug.Log("Starting to create arrow 2");
        currentActivePlacementTool = tool2Part_path;
        //currentActivePlacementTool.SetPlacementCompleteCallback(OnToolDone);
        currentActivePlacementTool.gameObject.SetActive(true);
        //Debug.Log("Starting to create arrow 3");
        currentActivePlacementTool.DoStart(tool2Part_path.OVERRIDE_USETHISASPREFAB);
        //Debug.Log("Starting to create arrow 4");

    }

    public void OnPlayerHasJustMadeThis(GameObject o)
    {
        PlayerCreatedPrefab pcp = o.GetComponent<PlayerCreatedPrefab>();
        if (pcp != null)
        {



            ///////////////////OLD STUFF//////////////////////////
            ///
            // is just creating a prefab ?
            
            //bool is2part = false;

            /**** TODO:LATER: FIGURE OUT HOW TO ADAPT THE POINTER TO EACH OBJECT
            if (pcp.myPrefabInfo.modelPrefabName == "Arrow" || pcp.myPrefabInfo.modelPrefabName == "/Arrow")
                is2part = true;
            ***/

            /**
            if (is2part)
            {
                // this object can handle 2-part creation, like arrow or tooltip

                Debug.Log("************* 2 part object !");
                if (currentActivePlacementTool != null)
                {
                    Debug.LogError("Should not have an active tool right now");
                }
                currentActivePlacementTool = tool2Part;
                currentActivePlacementTool.SetPlacementCompleteCallback(OnToolDone);
                currentActivePlacementTool.DoStart(pcp.gameObject);

                currentState = State.Positioning;
            }

            if (!is2part)
            {
                // object is a one-part creation process

                // TODO:come back in case we want to hold objects when they're created
                // for now just leave it alone 
                
                return;

                /////////////////////////////
                ///not run
                if (currentActivePlacementTool != null)
                {
                    Debug.LogError("Should not have an active tool right now");
                }
                currentActivePlacementTool = tool1Part;
                currentActivePlacementTool.SetPlacementCompleteCallback(OnToolDone);
                currentActivePlacementTool.DoStart(pcp.gameObject);

                currentState = State.Positioning;
            }
            **/
        } else
        {
            // we've created some other type of object; do nothing
        }
    }

    public void OnToolDone(bool byForce = false)
    {
        if (byForce)
        {
            currentActivePlacementTool.DoCancel();
        }
        currentActivePlacementTool.gameObject.SetActive(false); 
        currentActivePlacementTool = null;
        currentState = State.Idle;
    }

    // Start is called before the first frame update
    void Awake()
    { 
        Debug.Assert(!Instance, "Can't have multiple of these objects");
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(currentState + " " + TEMP_Debug_Pointers.Instance.pointerButtonSimulator.NowClicked);
        if (currentState == State.Positioning)
        {
            if (TEMP_Debug_Pointers.Instance.pointerButtonSimulator.NowClicked)
            {
                currentActivePlacementTool.DoClicked();
            }
        }



        /**
        if (TEMP_Debug_Pointers.Instance.pointerButtonSimulator.NowClicked)
        {
            foreach (var v in DEBUG_CLICKOTHERTOOLS)
            {
                if (v.isActiveAndEnabled && v != (Object)currentActivePlacementTool)
                    v.DoClicked();
            }
        }
        **/
    }
}
