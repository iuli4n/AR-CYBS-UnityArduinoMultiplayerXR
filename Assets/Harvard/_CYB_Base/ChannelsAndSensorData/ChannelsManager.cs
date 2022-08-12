
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChannelsManager : MonoBehaviour
{
    public static ChannelsManager Instance;
    Dictionary<string, ChannelModelHolder> channelToHolderMap;

    public string TODOREMOVE_currentChannelSelection;
    public GameObject currentComponent;

    // Start is called before the first frame update
    void Awake()
    {
        Debug.Assert(Instance == null, "Should not have multiple instances of this object!");
        Instance = this;

        channelToHolderMap = new Dictionary<string, ChannelModelHolder>();

        foreach (ChannelModelHolder h in gameObject.GetComponentsInChildren<ChannelModelHolder>())
        {
            //Debug.Log("ChannelsManager: Found channel " + h.channelName);
            channelToHolderMap.Add(h.channelName, h);
        }
    }

    public AtomicDataModel GetModelForChannel(string channelName)
    {
        try
        {
            return channelToHolderMap[channelName].model;
        }
        catch (KeyNotFoundException)
        {
            Debug.LogError("ChannelsManager: No model exists for channel '" + channelName +"'");
            return null;
        }
    }
    public string[] GetChannelNames()
    {
        string[] keys = new string[channelToHolderMap.Keys.Count];
        channelToHolderMap.Keys.CopyTo(keys, 0);
        return keys;
    }

    public void ChannelSwitch(string channelName)
    {
        TODOREMOVE_currentChannelSelection = channelName;
        currentComponent.GetComponent<AtomicDataSwitch>().SetCurrentChannel(TODOREMOVE_currentChannelSelection);
    }
}
