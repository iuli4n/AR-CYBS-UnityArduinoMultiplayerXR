using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InGameDebugConsole : MonoBehaviour
{
    public TextMeshPro textMesh;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC] void RPC_DebugLog(string text)
    {
        string newText = textMesh.text + "\n" + text;
        Debug.LogWarning(newText.Split('\n').Length);
        if (newText.Split('\n').Length > 15)
        {
            // cut the first few lines
            newText = newText.Substring(newText.IndexOf("\n")+1);
        }
        textMesh.text = newText;
    }

    private void OnGUI()
    {
        if (GUILayout.Button("TEST TEXT"))
        {
            string text = "blah blah";
            PhotonView.Get(this).RPC("RPC_DebugLog", RpcTarget.AllViaServer, text); ;
        }
    }
}
