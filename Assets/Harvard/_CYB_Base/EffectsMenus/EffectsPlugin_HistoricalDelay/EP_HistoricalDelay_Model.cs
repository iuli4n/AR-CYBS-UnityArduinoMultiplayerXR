using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EP_HistoricalDelay_Model : AEffect4PluginModel
{

    public bool isEnabled = false;
    public float delayMillis = 1000f;

    override public void DoPhotonSerialize(bool isWriting, PhotonStream stream)
    {
        if (isWriting)
        {
            stream.SendNext(isEnabled);
            stream.SendNext(delayMillis);

        } else
        {
            isEnabled = (bool)stream.ReceiveNext();
            delayMillis = (float)stream.ReceiveNext();
        }
    }
    
}
