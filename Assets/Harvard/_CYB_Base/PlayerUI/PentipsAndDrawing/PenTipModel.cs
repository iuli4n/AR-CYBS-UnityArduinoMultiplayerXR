using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

using UnityEngine;

// TODO: Rename this because it's just a atomic string model

public class PenTipModel : MonoBehaviour, IPunObservable
{
    //[ReadOnly]
    public string _lastTriggerEntered;



    // ========= AUTOGENERATESTART ======== Generated on 7/22/2020 4:32:31 PM ========= 


    public String lastTriggerEntered
    {
        get { return _lastTriggerEntered; }
        set { if (_lastTriggerEntered != value) { _lastTriggerEntered = value; /****OnLocalModelChanged();***/ modelChangedLastTriggerEntered?.Invoke(_lastTriggerEntered); } }
    }

    private const int fieldID_lastTriggerEntered = 1;
    public delegate void LastTriggerEnteredChangeDelegate(String value);
    public event LastTriggerEnteredChangeDelegate modelChangedLastTriggerEntered = delegate { };

    #region IPunObservable implementation

    // Photon will call this to synchronize the model data
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // Note, this is called both when reading or writing

        if (stream.IsWriting)
        {
            //if (needsNetworkSync)
            {
                stream.SendNext(_lastTriggerEntered);
                //needsNetworkSync = false;
            }
        }
        else
        {
            this._lastTriggerEntered = (string)stream.ReceiveNext();
            modelChangedLastTriggerEntered?.Invoke(_lastTriggerEntered);
        }

    }

    #endregion 

    /*****
    public override void SerializeModelFull(Stream outStream, NetworkedModelFieldSerializer formatter)
    {

        formatter.Serialize(outStream, fieldID_lastTriggerEntered);
        formatter.Serialize(outStream, _lastTriggerEntered);

    }

    public override void DeserializeModelFull(Stream inStream, NetworkedModelFieldSerializer formatter)
    {

        Debug.Assert(fieldID_lastTriggerEntered == (int)formatter.Deserialize(inStream));
        _lastTriggerEntered = (String)formatter.Deserialize(inStream);
        modelChangedLastTriggerEntered?.Invoke(_lastTriggerEntered);

    }
    ******/

    // ========= AUTOGENERATEEND ======== Generated on 7/22/2020 4:32:31 PM ========= 


}
