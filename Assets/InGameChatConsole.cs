using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InGameChatConsole : MonoBehaviour
{
    static public InGameChatConsole Instance;

    public TextMeshPro textMesh;

    // Start is called before the first frame update
    void OnEnable()
    {
        Debug.Assert(Instance == null, "Found another instance of this object but there should only be one in scene!");

        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC] void RPC_DebugLog(string text)
    {
        AddLogMessage(text, false);
    }

    private void AddLogMessage(string text, bool isPrivate)
    {
        string newMessage = (isPrivate? "[local] ":"") + text;

        string newText = textMesh.text + "\n" + newMessage;
        if (newText.Split('\n').Length > 15)
        {
            // our buffer is too big, cut the first line
            newText = newText.Substring(newText.IndexOf("\n") + 1);
        }
        textMesh.text = newText;

        Debug.Log("ChatConsole New Message: " + newMessage);
    }

    // Post log message for everyone in the network
    public void PostMessage(string text)
    {

        PhotonView.Get(this).RPC("RPC_DebugLog", RpcTarget.AllViaServer,
            "[" + (PhotonNetwork.LocalPlayer.ActorNumber) + "] "+
            text);

    }

    // Post log message only for me
    public void PostMessageLocal(string text)
    {
        AddLogMessage(text, true);
    }

    private void OnGUI()
    {
        if (GUILayout.Button("TEST TEXT"))
        {
            string text = "blah blah";
            PostMessage(text);
        }
    }
}
