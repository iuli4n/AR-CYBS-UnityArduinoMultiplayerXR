using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tool_Creator1Part : MonoBehaviour
{
    public GameObject prefab;

    public GameObject manipulatedObject;
    

    public UnityAction OnPlacementComplete;


    public enum State
    {
        Idle,
        PlacingFirst,
        PlacingSecond
    }
    public State currentState;

    // Start is called before the first frame update
    void Start()
    {
        currentState = State.Idle;
        UpdatePointerToCurrentState();
    }

    public void Click0_PlaceBasic(Vector3 startPos)
    {
        // TODOIR: Needs testing: Instead of _Special, this used to be _ModelFromPrefab
            manipulatedObject = SceneStepsManager.Instance.CreateAndConfigureNewObjectForScene_Special(
                prefab.name,
                "InternalPrefabs/" + prefab.name,
                startPos, Quaternion.identity, prefab.transform.localScale);

        
    }
    public void Click1_PlaceFirst(Vector3 clickPos)
    {
        manipulatedObject.transform.position = clickPos;
    }
    public void Click2_PlaceSecond(Vector3 clickPos)
    {
        manipulatedObject.transform.position = clickPos;
    }

    void MoveToNextState(Vector3 pointerLocation)
    {
        switch (currentState)
        {
            case State.Idle:
                Click0_PlaceBasic(pointerLocation);
                currentState = State.PlacingSecond; /***!!!**/
                break;
            case State.PlacingFirst:
                Click1_PlaceFirst(pointerLocation);
                currentState = State.PlacingSecond;
                break;
            case State.PlacingSecond:
                Click2_PlaceSecond(pointerLocation);

                OnPlacementComplete?.Invoke();

                manipulatedObject = null;
                currentState = State.Idle;
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
            case State.Idle:
                TEMP_Debug_Pointers.Instance.SetEditingPointerHead(Color.white);
                break;
            case State.PlacingFirst:
                TEMP_Debug_Pointers.Instance.SetEditingPointerHead(Color.green);
                break;
            case State.PlacingSecond:
                TEMP_Debug_Pointers.Instance.SetEditingPointerHead(Color.magenta);
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(currentState);

        if (TEMP_Debug_Pointers.Instance.pointerButtonSimulator.NowClicked)
        {
            MoveToNextState(TEMP_Debug_Pointers.Instance.currentPointerTipPoint.transform.position);
        }




        if (currentState == State.PlacingFirst)
        {
            manipulatedObject.transform.position = (TEMP_Debug_Pointers.Instance.currentPointerTipPoint.transform.position);
        }
        if (currentState == State.PlacingSecond)
        {
            manipulatedObject.transform.position = (TEMP_Debug_Pointers.Instance.currentPointerTipPoint.transform.position);
        }
    }
}
