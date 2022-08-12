using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EP_RandomScale_Model : AEffect4PluginModel
{

    public bool isEnabled = false;
    public float randomMagnitude = 5f;

    override public void DoPhotonSerialize(bool isWriting, PhotonStream stream)
    {
        if (isWriting)
        {
            stream.SendNext(isEnabled);
            stream.SendNext(randomMagnitude);

        } else
        {
            isEnabled = (bool)stream.ReceiveNext();
            randomMagnitude = (float)stream.ReceiveNext();
        }
    }
    
}
