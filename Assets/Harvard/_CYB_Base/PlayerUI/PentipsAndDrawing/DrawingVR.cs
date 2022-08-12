using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.UI;


public class PenScript : MonoBehaviour//PunCallbacks
{
    //[ReadOnly]
    public ActivateTip penTipManager;

}

public class DrawingVR : PenScript
{
    public GameObject lineDrawingPrefab;

    // TODO:TODO: Remove this and use the line object's colormodel
    public AtomicDataModel penColorModel;
    private Material lastUsedColor;
    public Material lineMaterial_color0;
    public Material lineMaterial_color1;
    public Material lineMaterial_color2;
    public Material lineMaterial_color3;


    /*
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    */

    
    // the current line being drawn (if not drawing then this is null)
    public PlayerCreatedDrawing lineManager;
    

    public GameObject penTip;
    private GameObject targetQuad;

    private int buttonValue;

    private bool isPressed;

    public GameObject TEMP_DrawingsAnchorParent;



    // Start is called before the first frame update
    void Start()
    {
        //lastColor = Color.white;
        lastUsedColor = lineMaterial_color0;
        LocalDraw_Initialize();

        penColorModel.OnDataUpdated += ColorModel_OnUpdated;
    }



    private float hack_nextLineBufferUpdate;

    void LocalDraw_Initialize()
    {
        //Debug.LogWarning("** DRAW localdraw_initialize");

        //recentDrawingBuffer = this.gameObject.GetComponent<NetworkedListBufferModel_Vector3>();
        //recentDrawingBuffer.modelChangedMyList += RemoteDraw_BufferChanged;


        //myLineBuffer = new LineBuffer();
        //myLineBuffer.onBufferFull += RemoteDraw_LineBufferIsFull;
    }

    
    void LocalDraw_Tick()
    {
        // nothing, the model updates itself
        //if (!myLineBuffer.IsEmpty && hack_nextLineBufferUpdate < Time.time) { myLineBuffer.FireBufferFull(); myLineBuffer.Clear(); }
    }



    float lastPointAddedAtTime;




    void UI_PenJustPressed()
    {
        // we've just pressed this button - so make new line
        isPressed = true;

        // snap the drawing tip to the location of the pen so it doesn't drag ink all over the scene
        targetQuad.transform.position = penTip.transform.position;

        // Create new DrawnLine line renderer object
        Local_CreateNewLineObject();
    }
    void UI_PenJustReleased()
    {
        // Just released
        isPressed = false;
        Local_ReleaseCurrentDrawing();
    }
    void UI_PenStillPressed()
    {
        lastPointAddedAtTime = Time.time;
        {
            //Debug.Log("** DRAW add point");


            if (!lineManager.IsConfigured())
                // wait until PCO is configured because linerenderer might be moved around
                return;

            // local we're holding down the button
            // draw points by adding to the buffer

            /*
            if (false)
            {
                Debug.Log(isPressed + "    " + line.transform.InverseTransformPoint(targetQuad.transform.position)
                    + "   " + line.transform.InverseTransformPoint(penTip.transform.position));
            }
            */

            lineManager.LocalDraw_AddPointToBuffer(targetQuad.transform.position);
        }
    }

    void ColorModel_OnUpdated(float newval)
    {
        // TODO: TODO: Use the color model that's on the line object directly, and remove this.

        // Someone changed the color
        if (!lineManager)
        {
            // we're not drawing anything
            return;
        }

        if (newval == 0)
            lineManager.SetLineMaterial(lineMaterial_color0); 
        if (newval == 1)
            lineManager.SetLineMaterial(lineMaterial_color1);
        if (newval == 2)
            lineManager.SetLineMaterial(lineMaterial_color2);
        if (newval == 3)
            lineManager.SetLineMaterial(lineMaterial_color3);

        lastUsedColor = lineManager.GetLineMaterial();
    }

    // Update is called once per frame
    void Update()
    {
        /*FAKEPHOT
        if (PhotonSimulatorIuli.Instance.USING_REAL_PHOTON ? !photonView.IsMine : false)
        { return; // ignore pen tips that aren't mine }
        */

        // COLORS 
        {
            //lastColor = this.GetComponent<SetActivePenTip>().colorPicked;
            //drawingParent = this.GetComponent<SetActivePenTip>().newDrawingParent;
            // TODO: PERFORMANCE: this should not happen here
            //lastColor = GetComponentInParent<ActivateTip>().colorPicked;
            ///drawingParent = GetComponentInParent<ActivateTip>().newDrawingParent;
        }

        // MOVE TARGET QUAD

        if (targetQuad == null)
        {
            if (penTipManager != null)
                targetQuad = penTipManager.GetComponent<ActivateTip>().targetQuad;
            else
            {
                Debug.LogError("Cannot be drawing without a targetQuad - exiting");
                return;
            }
        }
        targetQuad.transform.position = Vector3.Lerp(targetQuad.transform.position, penTip.transform.position, Time.fixedDeltaTime * 20);



        // FAKE MOUSE
        // Figure out what the (self or other person's) mouse is doing
        if (penTipManager.debug_Pen.simulatedButton1.Clicked)
        {
            buttonValue = 1;
        }
        if (!penTipManager.debug_Pen.simulatedButton1.Clicked)
        {
            buttonValue = 0;
        }


        if (!penTipManager.BelongsToLocalPlayer)
            // don't respond to networked player's drawings (they will instantiate drawing objects and populate them with drawing data through models)
            return;



        if (buttonValue == 1 && !isPressed)
        {
            // Just clicked
            UI_PenJustPressed();
            //FAKEPHOT PhotonView.Get(this).RPC("RPC_CreateNewLineObject", RpcTarget.All);
        }

        if (buttonValue == 1 && isPressed && (Time.time - lastPointAddedAtTime) > 0.005f)
        {
            // Still pressed
            if (lineManager)
            {
                UI_PenStillPressed();
            } else {
                // probably was deleted while we were editing, probably because the scene changed, so just ignore it
            }
        }

        else if (buttonValue == 0 && isPressed)
        {
            UI_PenJustReleased();
        }

    }

    public void Local_ReleaseCurrentDrawing()
    {
        // Called when the mouse is released;
        // this cleans up the line to have minimal # of points, and deletes it if it's too short 

        if (lineManager == null)
        {
            // not drawing anything so nothing to do
            return;
        }

        lineManager.FinalizeDrawing();
        lineManager = null;
    }


    //[PunRPC]
    public void Local_CreateNewLineObject()
    {
        // Creates a new line object for everyone. Then on every update we will add points to the model associated with this.

        Debug.Log("CreateNewLineObject");

        GameObject newLineObject = SceneStepsManager.Instance.CreateAndConfigureNewObjectForScene_BaseLineDrawing(
            SceneStepsManager.PREFABNAME_BASELINE, "InternalPrefabs/"+SceneStepsManager.PREFABNAME_BASELINE, 
            penTip.transform.position, Quaternion.identity, Vector3.one);

        lineManager = newLineObject.GetComponent<PlayerCreatedDrawing>();
        
        lineManager.SetLineMaterial(lastUsedColor);
        
    }


}



