using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AtomicDataModel : MonoBehaviourPun, IPunObservable
{
    public delegate void DataUpdatedEventHandler(float newValue);
    public event DataUpdatedEventHandler OnDataUpdated;

    //[SerializeField] public ArduinoComponents arduinoComponents;

    // When changing this model's value, can anyone change the value rely on photon ownership ? NOTE: if need to request photon ownership, this doesn't wait for a response
    // CONTRACT: This _autoTakeover doesn't change at runtime [it's not serialized]
    public bool _autoTakeover = true;

    // readonly, current value of this model
    public float _value;


    // HACK HACK HACK// HACK HACK HACK// HACK HACK HACK
    // HACK HACK HACK// HACK HACK HACK// HACK HACK HACK

    public float force_newValue;
    public bool forceIt = false;

    public virtual float Value
    {
        get { return _value; }
        set { if (value != _value)
            {
                if (!photonView.AmOwner)
                {
                    if (needsNetworkSync)
                    {
                        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
                        {
                            // already flagged so probably means we've already requested ownership, but we're still not the owner, grr
                            Debug.LogWarning("AtomicDataModel: Repeatedly attempting to change a model when we're not the owner, but this model is not being synchronized... this might indicate a problem with ownership or the photon network");
                        }
                    }

                    if (_autoTakeover)
                    {
                        photonView.RequestOwnership();
                    }
                    else
                    {
                        Debug.LogWarning("AtomicDataModel: Attempted to change value but we're not the owner; ignoring value change."); return;
                    }
                }

                // Change the local value even if we're not the owner and trigger updates
                _value = value; needsNetworkSync = true; InvokeOnDataUpdated(value);
            } 
        }
    }

    public void InvokeOnDataUpdated(float value)
    {
        OnDataUpdated?.Invoke(value);
    }

    private bool needsNetworkSync;








    void Start()
    {

    }

    void Update()
    {
        if (forceIt) this.Value = force_newValue;
    }
    #region IPunObservable implementation

    // Photon will call this to synchronize the model data
    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // Note, this is called both when reading or writing

        if (stream.IsWriting)
        {
            //if (needsNetworkSync)
            {
                stream.SendNext(_value);
                needsNetworkSync = false;
            }
        }
        else
        {
            this._value = (float)stream.ReceiveNext();
            //needsArduinoSync = true;
            InvokeOnDataUpdated(_value);
        }

    }

    #endregion //NetworkedModel
}
