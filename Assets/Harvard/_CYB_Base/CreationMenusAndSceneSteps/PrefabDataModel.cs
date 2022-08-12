using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PrefabDataModel : MonoBehaviourPun, IPunObservable
{
    // ASSUMES that nobody listens to data changes; otherwise we'll need a change listener

    public string modelPrefabName;

    public float scaleMultiplier = 1;


    #region IPunObservable implementation

    // Photon will call this to synchronize the model data
    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // Note, this is called both when reading or writing

        if (stream.IsWriting)
        {
            //if (needsNetworkSync)
            {
                stream.SendNext(modelPrefabName);
                stream.SendNext(scaleMultiplier);
                //needsNetworkSync = false;
            }
        }
        else
        {
            this.modelPrefabName = (string)stream.ReceiveNext();
            this.scaleMultiplier = (float)stream.ReceiveNext();
            //needsArduinoSync = true;
            //InvokeOnDataUpdated(_value);
        }

    }

    #endregion //NetworkedModel
}
