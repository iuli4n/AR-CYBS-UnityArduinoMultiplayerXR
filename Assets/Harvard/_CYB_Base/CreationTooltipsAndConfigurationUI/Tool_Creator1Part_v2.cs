using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tool_Creator1Part_v2 : IPositioningTool
{
    public GameObject prefab;

    public GameObject manipulatedObject;


    private UnityAction OnPlacementComplete;
    public override void SetPlacementCompleteCallback(UnityAction e)
    {
        OnPlacementComplete = e;
    }

    public enum State
    {
        NotAttachedToObject,
        //PreCreation,
        Positioning
    }
    public State currentState;

    // Start is called before the first frame update
    void Start()
    {
        currentState = State.NotAttachedToObject;

        //currentState = State.PreCreation;
        UpdatePointerToCurrentState();
    }

    /*
    public void Click0_PlaceBasic(Vector3 startPos)
    {
        manipulatedObject = SceneStepsManager.Instance.CreateAndConfigureNewObjectForScene_ModelFromPrefab(
            prefab.name,
            "InternalPrefabs/" + prefab.name,
            startPos, Quaternion.identity, prefab.transform.localScale);


    }
    */

    public void PlaceObject(Vector3 clickPos)
    {
        manipulatedObject.transform.position = clickPos;
    }

    void DoNextState(Vector3 pointerLocation)
    {
        switch (currentState)
        {
            case State.NotAttachedToObject:
                Debug.LogError("There's no next state for this tool. This tool should be disabled right now.");
                break;
            /*
            case State.PreCreation:
                Click0_PlaceBasic(pointerLocation);
                currentState = State.Positioning; 
                break;
            */
            case State.Positioning:
                PlaceObject(pointerLocation);

                OnPlacementComplete?.Invoke();

                manipulatedObject = null;
                currentState = State.NotAttachedToObject;
                break;
            default:
                break;
        }

        UpdatePointerToCurrentState();

    }

    void UpdatePointerToCurrentState()
    {
        switch (currentState)
        {
            /*
            case State.PreCreation:
                TEMP_Debug_Pointers.Instance.SetEditingPointerHead(Color.white);
                break;
            */
            case State.Positioning:
                TEMP_Debug_Pointers.Instance.SetEditingPointerHead(Color.magenta);
                break;
            default:
                break;
        }
    }


    private void LoopCurrentState()
    {
        //Debug.Log("tool2p2 state "+currentState);

        if (currentState == State.Positioning)
        {
            manipulatedObject.transform.position = (TEMP_Debug_Pointers.Instance.currentPointerTipPoint.transform.position);
        }
    }

    public override void DoClicked()
    {
        DoNextState(TEMP_Debug_Pointers.Instance.currentPointerTipPoint.transform.position);
    }

    public override void DoCancel()
    {
        if (manipulatedObject != null)
        {
            GameObject.Destroy(manipulatedObject);
            manipulatedObject = null;
        }

        currentState = State.NotAttachedToObject;
    }

    public override void DoStart(GameObject withThisObject)
    {
        manipulatedObject = withThisObject;
        currentState = State.Positioning;
    }


    // Update is called once per frame
    void Update()
    {
        LoopCurrentState();
    }
}
