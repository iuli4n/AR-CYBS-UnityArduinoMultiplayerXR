
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
//using System.Runtime.InteropServices;
using UnityEngine;

//namespace Harvard.ModelNetworking.Extras

// Note: The changes in the buffer are not synchronized; only synchronized if the whole buffer changes
public class ConstBytesBufferModel : MonoBehaviourPunCallbacks, IPunObservable //, IModelNetObservable
{
    // Data that is synchronized

    //[ReadOnly]
    private byte[] _bytes;



    // ========= AUTOGENERATESTART ======== Generated on 8/8/2020 5:40:11 PM ========= 

    //[ReadOnly]
    public bool needsSync = false;

    public Byte[] bytes
    {
        get { return _bytes; }
        set { if (_bytes != value) { _bytes = value; /*OnLocalModelChanged();*/ needsSync = true; modelChangedBytes?.Invoke(_bytes); } }
    }

    private const int fieldID_bytes = 1;
    public delegate void BytesChangeDelegate(Byte[] value);
    public event BytesChangeDelegate modelChangedBytes = delegate { };


    #region IPunObservable implementation

    public float nextUpdateTime; // ensures the model doesn't get written to the stream too often

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // called both when reading or writing

        if (stream.IsWriting)
        {
            // WRITING: we own this player so we send out this data : send the others our data

            //Debug.Log("WRITE: " + Marshal.UnsafeAddrOfPinnedArrayElement(_bytes, 0));

            if (needsSync)
            {
                if (nextUpdateTime >= Time.time) return;
                nextUpdateTime = Time.time + 0.1f;

                Debug.Log("WRITTEN"); 
                //stream.SendNext((long)Marshal.UnsafeAddrOfPinnedArrayElement(_bytes, 0));
                stream.SendNext(_bytes);
                needsSync = false;
            }
        }
        else
        {
            // READING from network, receive data from another client
            //long x = (long)stream.ReceiveNext();

            this._bytes = (byte[])stream.ReceiveNext();

            Debug.Log("READ NEW BYTES");
            modelChangedBytes?.Invoke(_bytes);

        }

    }

#if false
    public void OnModelNetSerialize(bool isWriting, Stream stream, NetworkedModelFieldSerializer serializer)
    {
        // called both when reading or writing

        if (isWriting)
        {
            // WRITING: we own this player so we send out this data : send the others our data

            //Debug.Log("WRITE: " + Marshal.UnsafeAddrOfPinnedArrayElement(_bytes, 0));

            if (needsSync)
            {
                //stream.SendNext((long)Marshal.UnsafeAddrOfPinnedArrayElement(_bytes, 0));
                serializer.Serialize(stream, _bytes);
                needsSync = false;
            }
        }
        else
        {
            // READING from network, receive data from another client
            //long x = (long)stream.ReceiveNext();

            this._bytes = (byte[])serializer.Deserialize(stream);

            //Debug.Log("READ: " + x + "  //  " + Marshal.UnsafeAddrOfPinnedArrayElement(_bytes, 0));
            modelChangedBytes?.Invoke(_bytes);

        }

    }


#endif
#endregion //NetworkedModel


    // ========= AUTOGENERATEEND ======== Generated on 8/8/2020 5:40:11 PM ========= 
}
