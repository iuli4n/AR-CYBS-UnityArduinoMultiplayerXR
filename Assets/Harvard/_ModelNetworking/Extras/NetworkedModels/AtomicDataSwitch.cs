using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtomicDataSwitch : AtomicDataModel
{
    // readonly - changes not synchronized; shouldn't change during runtime
    public string switchLabel;

    // ONLY FOR THE UNITY INSPECTOR
    public string unityView__CurrentChannel = "C2"; // only for the unity inspector debugging !

    [SerializeField]
    private string _currentChannel;
    [SerializeField] 
    private bool needsNetworkSync2;

    [SerializeField] 
    private AtomicDataModel actualDataModel = null;

#if UNITY_EDITOR
    public float debug_ActualValue;
#endif

    public override float Value
    {
        get { return actualDataModel != null ? actualDataModel.Value : -999; }
        set
        {
            // someone is attempting to set the value of the model inside this switch, through the switch
            // so just update the local model that we're holding
            UpdateLocalModel(value);
        }
    }



    public string CurrentChannel { get => _currentChannel; 
        set {
            if(value != _currentChannel)
                if (!photonView.AmOwner) { if (_autoTakeover) { photonView.RequestOwnership(); SetCurrentChannel(value, false); } else { Debug.LogWarning("AtomicDataModelSwitch: Attempted to change channel value but we're not the owner; ignoring value change."); return; } }
                else { SetCurrentChannel(value, false); } } }



    public void MoveToNextChannel()
    {
        string[] availableChannels = ChannelsManager.Instance.GetChannelNames();

        int nextIndex = -1;
        int currentIndex = 0;
        foreach (string c in availableChannels)
        {
            if (_currentChannel.Equals(c))
            {
                nextIndex = (currentIndex + 1) % availableChannels.Length;
                break;
            }
            currentIndex++;
        }
        if (nextIndex == -1)
        {
            // we couldn't find our channel; this shouldn't happen but if it does just choose the first one
            nextIndex = 0;
        }
        CurrentChannel = availableChannels[nextIndex];
    }

    public bool SetCurrentChannel(string newChannel, bool updatedFromNetwork = false)
    {
        //Debug.Log("ADS SetCurrentChannel " + newChannel);

        AtomicDataModel m = ChannelsManager.Instance.GetModelForChannel(newChannel);
        if (m == null)
        {
            Debug.LogError("ChannelsManager could not find model for channel " + newChannel);
            return false;
        }

        // Detatch from current model
        if (actualDataModel != null)
        {
            actualDataModel.OnDataUpdated -= OnInternalModelChanged;
        }
        // Attach to new model
        actualDataModel = m;
        actualDataModel.OnDataUpdated += OnInternalModelChanged;

        if (!updatedFromNetwork) 
            needsNetworkSync2 = true;

        _currentChannel = newChannel;
        
        unityView__CurrentChannel = _currentChannel;

        InvokeOnDataUpdated(Value);
        return true;
    }

    private void OnDestroy()
    {
        //Debug.LogWarning("DESTROYING ADS");

        if (actualDataModel != null)
            actualDataModel.OnDataUpdated -= OnInternalModelChanged;
    }

    private void UpdateLocalModel(float newData)
    {
        // This means someone called (this.value = x), meaning they changed the value on our copy of the model;
        // they didn't change the value on the original mapped model, so we need to do that
        if (actualDataModel != null && actualDataModel.Value != newData)
            actualDataModel.Value = newData;
        // this will trigger an update that would come back through us
    }
    private void OnInternalModelChanged(float newData)
    {
        // the data from the original mapped model was changed. set our copied model's data, which will cause an event cascade to anyone listening to us
        this.Value = newData;
        //Debug.Log("ADS: internal model updated to new value "+newData);
        InvokeOnDataUpdated(Value);
    }

    // Start is called before the first frame update
    void Start()
    {
        //this.OnDataUpdated += OnLocalModelUpdated; // listener that responds to changes made to us

        /*
         * if (_currentChannel != "")
        {
            SetCurrentChannel(_currentChannel);
        }
        */


        if (actualDataModel == null && CurrentChannel != "")
        {
            // we already have a channel but is probably the first time we're running
            SetCurrentChannel(CurrentChannel);
        }
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        debug_ActualValue = Value;
#endif

        if (unityView__CurrentChannel != CurrentChannel && unityView__CurrentChannel != "")
        {
            SetCurrentChannel(unityView__CurrentChannel);
        }
    }
    private void OnGUI_OLDER()
    {
        // DEBUGGING ONLY

        GUILayout.BeginVertical();

        GUILayout.Label(".");
        GUILayout.Label(".");
        GUILayout.Label(".");
        GUILayout.Label(".");
        GUILayout.Label(".");
        GUILayout.Label(".");
        GUILayout.Label(".");
        if (GUILayout.Button("channel C1"))
        {
            SetCurrentChannel("C1");
        }
        if (GUILayout.Button("channel C2"))
        {
            SetCurrentChannel("C2");
        }
        if (GUILayout.Button("channel C3"))
        {
            SetCurrentChannel("C3");
        }

        GUILayout.EndVertical();
    }


    // override the serialization - this object just serializes its channel name
    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //Debug.Log("ADS photon serialize " + stream.IsWriting + needsNetworkSync2 + CurrentChannel);

        // Note, this is called both when reading or writing

        if (stream.IsWriting)
        {
            if (needsNetworkSync2)
            {
                stream.SendNext(true);
                stream.SendNext(CurrentChannel);
                needsNetworkSync2 = false;
            } else
            {
                stream.SendNext(false);
            }
        }
        else
        {
            bool hasdata = (bool)stream.ReceiveNext();
            Debug.Log("ADS: Network has data " + hasdata);

            if (hasdata)
            {
                string s = (string)stream.ReceiveNext();
                Debug.Log("ADS: Network received " + s);
                SetCurrentChannel(s, true);
            }
            
        }

    }
}
