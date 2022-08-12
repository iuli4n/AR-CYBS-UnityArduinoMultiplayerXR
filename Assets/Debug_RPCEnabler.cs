using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_RPCEnabler : MonoBehaviourPun
{
    public bool showGUI;
    public GameObject[] objects;

    public void EnableObject(int index)
    {
        EnableObject(index, true);
    }
    public void EnableObject(int index, bool disableOthers)
    {
        PhotonView.Get(this).RPC("RPC_EnableObject", RpcTarget.All, index, disableOthers);
    }
    [PunRPC]
    public void RPC_EnableObject(int index, bool disableOthers)
    {
        if (index == -1 || disableOthers)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                objects[i].SetActive(i == index);
            }
        } else {
            objects[index].SetActive(true);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnGUI()
    {
        if (!showGUI) return;

        GUILayout.Label("");
        GUILayout.Label("");
        GUILayout.Label("");

        for (int i = 0; i < objects.Length; i++)
        {
            GameObject g = objects[i];
            if (GUILayout.Button(g.name +" "+(g.active?"(E)":"(D)")))
            {
                EnableObject(i, true);
            }
        }
        /*
        if (GUILayout.Button("ALL D"))
        {
            EnableObject(-1, true);
        }
        */
    }
}
