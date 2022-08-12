using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DebugUI_UndoRedo : MonoBehaviour
{


    // NEXT STEPS:
    // -- possible bug: if DrawingVR doesn't trigger this properly, then undo/redo states may be invalid


    /**
    public static DebugUI_UndoRedo Instance;

    // Start is called before the first frame update
    DebugUI_UndoRedo()
    {
        Debug.Assert(Instance == null, "Should not have multiple instances of this script");
        Instance = this;
    }
    ***/

    // Update is called once per frame
    void Update()
    {
        
    }



    private Stack<GameObject> undo_undoList = new Stack<GameObject>();
    private Stack<GameObject> undo_redoList = new Stack<GameObject>();

    /*
    private void Check_UndoRedo()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            UndoRedo_Undo();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            UndoRedo_Redo();
        }

    }
    */
    public void UndoRedo_AddNewObject(GameObject o)
    {
        Debug.Log("--undoredo added new object--");

        undo_undoList.Push(o);
        // clear the redo list since we're taking a new action
        foreach (GameObject ob in undo_redoList)
        {
            GameObject.Destroy(ob);
        }
        undo_redoList.Clear();
    }
    
    public void UndoRedo_Undo()
    {
        PhotonView.Get(this).RPC("RPC_UndoRedo_Undo", RpcTarget.All);
    }
    [PunRPC]
    private void RPC_UndoRedo_Undo() { 
        Debug.Log("--undo called--");

        if (undo_undoList.Count == 0) return;
        GameObject last = undo_undoList.Pop();
        // disable and add to redo list in case we want to redo
        last.SetActive(false);
        undo_redoList.Push(last);
    }

    public void UndoRedo_Reset()
    {
        PhotonView.Get(this).RPC("RPC_UndoRedo_Reset", RpcTarget.All);
    }
    [PunRPC]
    private void RPC_UndoRedo_Reset()
    {
        undo_undoList.Clear();
        undo_redoList.Clear();
    }
    
    public void UndoRedo_Redo()
    {
        PhotonView.Get(this).RPC("RPC_UndoRedo_Redo", RpcTarget.All);
    }
    [PunRPC]
    private void RPC_UndoRedo_Redo()
    {
        Debug.Log("--redo called--");

        if (undo_redoList.Count == 0) return;

        GameObject last = undo_redoList.Pop();
        // enable and add to undo list
        last.SetActive(true);
        undo_undoList.Push(last);
    }

}
