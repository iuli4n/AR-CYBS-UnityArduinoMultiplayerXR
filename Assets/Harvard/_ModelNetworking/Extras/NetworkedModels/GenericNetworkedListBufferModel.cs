using JetBrains.Annotations;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// This is a simple model that just holds a list which is updated according to a set timer, and after being updated it clears itself.
/// The model does its own update and triggers sync&clear events at the specified frequency.
/// 
/// Note, type T needs to be serializable through BinaryFormatter
/// </summary>
/// <typeparam name="T"></typeparam>
public class GenericNetworkedListBufferModel<T> : MonoBehaviourPunCallbacks, IPunObservable // NetworkedModel
{
    // this stores the actual list
    private NetworkedList<T> _myList;
    public NetworkedList<T> myList { get { return _myList; } }

    // callbacks when the list changes (ie: is synchronized/emptied)
    public delegate void MyListChangeDelegate(NetworkedList<T> newValue);
    public event MyListChangeDelegate modelChangedMyList = delegate { };

    // how often should this list be emptied/synchronized ?
    public float syncInterval = 0.1f; // number of seconds between synchronizations
    public float nextSyncTime = 0;
    public bool needsSync = false;


    //set { if (_allAvatars != value) { _allAvatars = value; OnLocalModelChanged(); modelChangedAllAvatars?.Invoke(_allAvatars); } }
    //private const int fieldID_myList = 1;
    



    public void Awake()
    {
        _myList = new NetworkedList<T>();
        _myList.onListChanged += LocalListChanged;
    }

    /// <summary>
    /// Triggered when the list model changed. This is either automatically when someone local accessed it (islocal=true), or when updated via photon (islocal=false)
    /// </summary>
    /// <param name="isLocalChange"></param>
    void LocalListChanged(bool isLocalChange)
    {
        // if it's a remote change, Deserialize already called the general callback for this list, so we don't need to do anything else
        // if it's a local change, we'll need to raise the general callback, but will be done later when the list is full enough

        if (isLocalChange)
        {
            needsSync = true;
        }
        
    }

    private void Update()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1 && needsSync)
        {
            // we're the only person in the room
            //
            // **** does what OnSerializePhotonView would do

            // reset update flags
            needsSync = false;
            nextSyncTime = Time.time + syncInterval;

            // call anyone who needs to know that we've just received something
            modelChangedMyList?.Invoke(_myList);

            // clear the list and raise the callbacks again
            _myList.Clear(); // this will set needsSync to true, so there will be another message with an empty list; or with other updates
            modelChangedMyList?.Invoke(_myList);
        }



        /******* this doesn't run because updates are driven by photon
        if (PhotonSimulatorIuli.Instance.USING_REAL_PHOTON) return; // updates driven by photon 

        if (needsSync && nextSyncTime < Time.time)
        {
            //older OnLocalModelChanged();
            modelChangedMyList?.Invoke(_myList);
            //older TODOIRNOW:putback SendToServerNow();

            needsSync = false;
            nextSyncTime = Time.time + syncInterval;

            _myList.Clear(); // this will set needsSync to true, so there will be another message with an empty list; or with other updates
            modelChangedMyList?.Invoke(_myList);
            //older SendToServerNow();
        }
        **********/
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // called both when reading or writing

        if (stream.IsWriting)
        {
            if (needsSync && nextSyncTime < Time.time)
            {
                // send the underlying list
                myList.Do_OnPhotonSerializeView(stream, info);

                // reset update flags
                needsSync = false;
                nextSyncTime = Time.time + syncInterval;

                // call anyone who needs to know that we've just received something
                modelChangedMyList?.Invoke(_myList);

                // clear the list and raise the callbacks again
                _myList.Clear(); // this will set needsSync to true, so there will be another message with an empty list; or with other updates
                modelChangedMyList?.Invoke(_myList);

                Debug.Log("**** NetworkedListBufferModel WROTE VIEWID: " + photonView.ViewID);
            }
        }
        else
        {
            myList.Do_OnPhotonSerializeView(stream, info);
            
            modelChangedMyList?.Invoke(_myList);

            Debug.Log("**** NetworkedListBufferModel READ VIEWID: " + photonView.ViewID);
        }

    }
}
