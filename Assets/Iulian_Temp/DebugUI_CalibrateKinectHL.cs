using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using Photon.Pun;
using Photon.Realtime;
using System;

public class DebugUI_CalibrateKinectHL : MonoBehaviour
{
    public bool showGUI;
    public GameObject mrtk_playspace;
    public GameObject hl_fingertip;
    public GameObject virtual_point;

    public string calibrationKey = "z";
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    [PunRPC]
    void RPC_CalibratePlayer(int playerNumber)
    {
        InGameChatConsole.Instance.PostMessage("RPC_CalibratePlayer received for " + playerNumber +
            " and I am " + PhotonNetwork.LocalPlayer.ActorNumber);

        if (!PhotonNetwork.LocalPlayer.ActorNumber.Equals(playerNumber))
            return; // not for us

        PerformCalibrationHololensFinger();

    }

    private void OnGUI()
    {
        if (!showGUI) return;

#if UNITY_EDITOR
        // The GUI editor allows calibration of all users !

        GUILayout.BeginVertical();
        if (PhotonNetwork.PlayerListOthers.Length == 0) { GUILayout.Label("No other players besides me here"); }
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (GUILayout.Button("Calibrate player id  " + player.ActorNumber 
                + (PhotonNetwork.LocalPlayer.ActorNumber == player.ActorNumber ? " (me)" : "") + " " 
                + "["+ Enum.GetName(typeof(RuntimePlatform), player.CustomProperties["platform"]) +"]"
                ))
            {
                PhotonView.Get(this).RPC("RPC_CalibratePlayer", RpcTarget.AllViaServer, player.ActorNumber);
            }
        }
        GUILayout.EndVertical();
#endif
    }




    void PerformCalibrationHololensFinger()
    {
        InGameChatConsole.Instance.PostMessage("Performing calibration " + hl_fingertip);
        if (hl_fingertip == null)
        {
            hl_fingertip = PlayersManager.Instance.localPlayerHead.transform.Find("FingertipCollider").gameObject;
        }

        PerformCalibration(
            hl_fingertip.transform.position, hl_fingertip.transform.forward,
            virtual_point.transform.position, virtual_point.transform.forward);

        InGameChatConsole.Instance.PostMessage("Done calibration ");
    }

    // Moves the MRTK playspace so that the two source/dest points are aligned
    void PerformCalibration(Vector3 sourcePos, Vector3 sourceRotFwd, Vector3 destPos, Vector3 destRotFwd)
    {
        Vector3 v1 = destRotFwd;
        Vector3 v2 = sourceRotFwd;
        Debug.Log(v1 + "  //  " + v2);
        v1.y = 0;
        v2.y = 0;
        Quaternion rotationVector = Quaternion.FromToRotation(v2, v1);
        Debug.Log(rotationVector);
        mrtk_playspace.transform.rotation *= rotationVector;


        Vector3 moveVector = destPos - sourcePos;
        mrtk_playspace.transform.position += moveVector;

    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(calibrationKey))
        {
            PerformCalibrationHololensFinger();
        }
    }
}
