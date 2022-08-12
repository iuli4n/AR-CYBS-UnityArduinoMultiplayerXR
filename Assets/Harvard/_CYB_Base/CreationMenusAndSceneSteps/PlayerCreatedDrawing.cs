using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCreatedDrawing : MonoBehaviour
{
    [SerializeField]
    public GameObject drawingContainer;
    [SerializeField]
    public LineRenderer drawingLine;

    [SerializeField]
    private PlayerCreatedObject pco;


    private NetworkedListBufferModel_Vector3 activeDrawingBuffer;

    void Awake()
    {
        drawingContainer = this.gameObject.transform.Find("DrawingContainer").gameObject;
        drawingLine = drawingContainer.GetComponent<LineRenderer>();
        pco = this.GetComponent<PlayerCreatedObject>();

        activeDrawingBuffer = this.gameObject.GetComponent<NetworkedListBufferModel_Vector3>();
        activeDrawingBuffer.modelChangedMyList += DrawBufferChanged;
    }

    public bool IsConfigured()
    {
        return pco.isConfigured;
    }

    public void RecalculateBounds()
    {
        bool boundsEnabled = pco.GetComponent<BoundsControl>().enabled;
        
        pco.GetComponent<BoundsControl>().enabled = false;
        GameObject.DestroyImmediate(pco.GetComponent<BoxCollider>());

        pco.GetComponent<BoundsControl>().Target = drawingContainer;


        if (boundsEnabled)
        {
            // reenable it now. otherwise it will be refreshed whenever it reenables
            pco.GetComponent<BoundsControl>().enabled = true;
        }

    }

    public void FinalizeDrawing()
    {
        // this cleans up the line to have minimal # of points, and deletes it if it's too short 

        PhotonView.Get(this).RPC("RPC_FinalizeDrawing", RpcTarget.All);
    }
    [PunRPC]
    public void RPC_FinalizeDrawing()
    {
        // this cleans up the line to have minimal # of points, and deletes it if it's too short 

        drawingLine.Simplify(0.001f);

        // now recalculate the bounding box through the BoundsControl
        RecalculateBounds();

        Debug.Log("Releasing drawing");

        if (drawingLine.positionCount < 3)
        {
            // small lines shouldn't exist
            PhotonNetwork.Destroy(this.gameObject);
            return;
        }

        // this line is valid let it exist and add it to undo

        // TODO: UNDOREDO
        // penTipManager.undoRedoManager.UndoRedo_AddNewObject(line.gameObject);
        
    }


    public void CreateFromPositions(Vector3[] positions)
    {
        drawingLine.positionCount = positions.Length;
        drawingLine.SetPositions(positions);
    }

    public void SetLineMaterial(Material m)
    {
        drawingLine.material = m;
    }
    public Material GetLineMaterial()
    {
        return drawingLine.material;
    }

    public void LocalDraw_AddPointToBuffer(Vector3 worldPoint)
    {
        // A new point should be added to the line. Put it in the buffer, and let that update everyone including us
        
        activeDrawingBuffer.myList.Add(drawingLine.transform.InverseTransformPoint(worldPoint));
    }


    private void FromBuffer_AddLinePoints(NetworkedList<Vector3> points)
    {
        int originalPositionCount = drawingLine.positionCount;
        drawingLine.positionCount = originalPositionCount + points.Count;

        for (int i=0; i<points.Count; i++)
        {
            Vector3 targetPos = points[i]; //targetQuad.transform.position;
            drawingLine.SetPosition(originalPositionCount + i, targetPos);
        }
    }




    private void DrawBufferChanged(object x)
    {
        // Line points buffer has changed, probably because a remote person is still drawing it. Add the points to our own line
        FromBuffer_AddLinePoints(activeDrawingBuffer.myList);
    }


    void Update()
    {
        
    }
}
