using Com.MyCompany.MyGame;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO:
// - Merge this with ActivateTip (they're both just in charge of the tips of the pen)
// - Move the NetworkedListBufferModel because that's what's used for keeping track of draw locations

public class Debug_Pen : MonoBehaviour//Pun
{

    public AtomicDataModelBool mouseButtonModel;

    public IuliButtonSimulator simulatedButton1 = new IuliButtonSimulator();

    //public bool DEBUG_LISTMODEL = false;
    //public NetworkedListBufferModel_Vector3 debugModel;
    public bool tickingRunning = false;

    public ActivateTip penTip;

    // Start is called before the first frame update
    void Start()
    {
    
    }

    /*
     * debugging only
    private IEnumerator TickingList()
    {
        int i = 0;
        while (true)
        {
            i++;
            debugModel.myList.Add((Vector3.one * i));
            yield return new WaitForSeconds(0.2f);
        }
    }
    */



    /*** BYPASSED ?
    [PunRPC]
    void ButtonPushed(string buttonName, string forPlayer)
    {
        if (guiPlayerName != forPlayer) return;

        if (buttonName == "ANCHOR_ON")
        {
            penTip.DoActivatePin();
        }
        else if (buttonName == "DRAW_ON")
        {
            penTip.DoActivateDrawing();
        }
    }
    ***/

    /*
    [PunRPC]
    void MouseButton1StateChanged(bool clicked)
    {
        //if (guiPlayerName != forPlayer) return;

        simulatedButton1.OnButtonChange(clicked);
    }
    **/

    [PunRPC]
    void EraseAllLines()
    {
        // TODO:BUG: this will interfere with undo/redo

        // TODO: just delete things related to our drawing, not other line renderers
        LineRenderer[] lines = (LineRenderer[])GameObject.FindObjectsOfTypeAll(new LineRenderer().GetType());
        foreach (LineRenderer r in lines) {
            GameObject.Destroy(r.gameObject);
        };
    }


    // Update is called once per frame
    void Update()
    {
        /*
        if (penTip == null)
        {
            /// TODOIRNOW: PUT THIS BACK
            /**
            if (GameManager.Instance != null && GameManager.Instance.localPlayerHead != null)
            {
                penTip = GameManager.Instance.localPlayerHead.GetComponentInChildren<ActivateTip>();
            }
            ***
        }
        */

        /***
        if (DEBUG_LISTMODEL)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.Log("---------");
                foreach (Vector3 f in debugModel.myList) Debug.Log(f);

            }
            else if (PhotonNetwork.IsMasterClient && !tickingRunning)
            {
                StartCoroutine(TickingList());
                tickingRunning = true;
            }
        }
        ***/
    }

    //public bool ICONTROLDRAWING = true;

    /**** BYPASS?
    public void OnEvent_MenuSelected_Anchoring()
    {
        String s = "ANCHOR_ON";
        ButtonPushed(s, guiPlayerName);
        //was: this.photonView.RPC("ButtonPushed", RpcTarget.All, s, guiPlayerName);
    }
    public void OnEvent_MensuSelected_Drawinsg()
    {
        String s = "DRAW_ON";
        ButtonPushed(s, guiPlayerName);
        //was: this.photonView.RPC("ButtonPushed", RpcTarget.All, s, guiPlayerName);
    }
    ***/

    private void OnGUI()
    {
        // TODO: Maybe should be taken to Update, because ongui happens multiple times per frame
        simulatedButton1.Tick();
        simulatedButton1.OnButtonChange(mouseButtonModel.Value); 
        GUILayout.Label("" + simulatedButton1.Clicked);
        //Debug.Log(simulatedButton1.Clicked);
        // KEYBOARD Z used to draw (the key is sent through the simulated button)
        /**
        if (ICONTROLDRAWING && 
                Input.GetKey(KeyCode.Z) != simulatedButton1.Clicked)
            MouseButton1StateChanged(Input.GetKey(KeyCode.Z));
        //this.photonView.RPC("MouseButton1StateChanged", RpcTarget.All, Input.GetKey(KeyCode.Z), guiPlayerName);
        **/

        if (penTip == null) return;

        
        



        /**** OLDER

        GUILayout.BeginArea(new Rect(300, 0, 300, 600));

        //if (Input.GetMouseButton(0) != simulatedButton1.Clicked) { this.photonView.RPC("MouseButton1StateChanged", RpcTarget.All, Input.GetMouseButton(0)); };

        if (GUILayout.Button("ANCHOR_ON_FORALL"))
        {
            OnEvent_MenuSelected_Anchoring();
        }
        if (GUILayout.Button("DRAW_ON_FORALL"))
        {
            
        }
        if (GUILayout.Button("ERASE LINES"))
        {
            EraseAllLines();
            //was: this.photonView.RPC("EraseAllLines", RpcTarget.All);
        }

        GUILayout.EndArea();
        *****/
    }
}
