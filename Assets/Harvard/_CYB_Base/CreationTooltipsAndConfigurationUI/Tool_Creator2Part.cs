using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class IPositioningTool : MonoBehaviour
{
    public abstract void DoStart(GameObject withThisObject);
    public abstract void DoClicked();
    public abstract void DoCancel();

    public abstract void SetPlacementCompleteCallback(UnityAction e);
}

public class Tool_Creator2Part : IPositioningTool
{
    // TODO: Check if OnEnable/OnDisable work properly when not set to start automatically
    //      TODO:PREVIOUSLY: Need OnPlacementCancelled

    public bool SkipPlacementStep = false;

    public GameObject prefab;

    public GameObject OVERRIDE_USETHISASPREFAB;

    public GameObject manipulatedObject;
    public EndpointsStretcher manipulatedEndpoints;


    private UnityAction OnPlacementComplete;
    public override void SetPlacementCompleteCallback(UnityAction e)
    {
        OnPlacementComplete = e;
    }

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
        //currentState = State.Idle;
        UpdatePointerToCurrentState();
    }

    private void ConfigureBase(Vector3 startPos)
    {
        if (!OVERRIDE_USETHISASPREFAB)
        {
            // TODONOW: This should be done over network (see comment below)
            //manipulatedObject = PhotonNetwork.Instantiate((GameObject)Resources.Load("InternalPrefabs/" + prefab.name));

            manipulatedObject = SceneStepsManager.Instance.CreateAndConfigureNewObjectForScene_Special(
                prefab.name,
                "InternalPrefabs/" + prefab.name,
                startPos, Quaternion.identity, prefab.transform.localScale);
            
        }
        //GameObject.Instantiate(prefab, startPos, Quaternion.identity);
        else
        {
            manipulatedObject = OVERRIDE_USETHISASPREFAB;
        }

        manipulatedEndpoints = manipulatedObject.GetComponentInChildren<EndpointsStretcher>();
        Debug.Log("*** ENDPOINTS FOUND: " + manipulatedEndpoints);


    }
    private void PlaceStartPoint(Vector3 clickPos)
    {
        manipulatedEndpoints.MoveStartPoint(clickPos);
    }
    private void PlaceEndPoint(Vector3 clickPos)
    {
        manipulatedEndpoints.MoveEndPoint(clickPos);
    }

    private void DoNextState(Vector3 pointerLocation)
    {
        switch (currentState)
        {
            case State.Idle:
                Debug.LogWarning("Tool2Part: SHOULD GET RID OF THIS KIND OF INSTANTIATION");
                ConfigureBase(pointerLocation);
                // and now will be moving the start point around
                currentState = State.PlacingFirst;
                break;

            case State.PlacingFirst:
                PlaceStartPoint(pointerLocation);
                // now will be moving endpoint
                currentState = State.PlacingSecond;
                break;

            case State.PlacingSecond:
                PlaceEndPoint(pointerLocation);
                // and now we're done
                manipulatedEndpoints.DoneEditing();
                OnPlacementComplete?.Invoke();

                manipulatedEndpoints = null; manipulatedObject = null;
                currentState = State.Idle;
                break;
            default:
                break;
        }

        UpdatePointerToCurrentState();

    }

    private void LoopCurrentState()
    {
        if (currentState == State.PlacingFirst)
        {
            // startpoint is freely manipulated, move it and the endpoint together
            manipulatedEndpoints.MoveStartPoint(TEMP_Debug_Pointers.Instance.currentPointerTipPoint.transform.position);
            manipulatedEndpoints.MoveEndPoint(TEMP_Debug_Pointers.Instance.currentPointerTipPoint.transform.position + new Vector3(0, 0.1f, 0));
        }
        if (currentState == State.PlacingSecond)
        {
            manipulatedEndpoints.MoveEndPoint(TEMP_Debug_Pointers.Instance.currentPointerTipPoint.transform.position);
        }
    }

    private void UpdatePointerToCurrentState()
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

    public override void DoClicked()
    {
        DoNextState(TEMP_Debug_Pointers.Instance.currentPointerTipPoint.transform.position);
    }

    public void OnEnable()
    {
        DoCancel();
        //DoStart(OVERRIDE_USETHISASPREFAB);
    }
    private void OnDisable()
    {
        DoCancel();
    }

    public override void DoCancel()
    {
        if (manipulatedObject != null)
        {
            GameObject.Destroy(manipulatedObject);
            manipulatedObject = null;
        }

        manipulatedEndpoints = null;
        currentState = State.Idle;
    }

    public override void DoStart(GameObject withThisObject)
    {
        // this requires that we take over the pointer
        //Debug.LogWarning("Tool_Creator2Part: we are switching pointer");
        //EditingManager.Instance.SwitchPointerTo(TEMP_Debug_Pointers.PointerType.Create_PrecisionCreate);

        if (SkipPlacementStep)
        {
            Debug.Log("** skipping first step **");
            OVERRIDE_USETHISASPREFAB = withThisObject;
            ConfigureBase(TEMP_Debug_Pointers.Instance.currentPointerTipPoint.transform.position);
            currentState = State.PlacingFirst;
        }
        else
        {
            currentState = State.Idle;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(currentState);

        LoopCurrentState();

    }
}
