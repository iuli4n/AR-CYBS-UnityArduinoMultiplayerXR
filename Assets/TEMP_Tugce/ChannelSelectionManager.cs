using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChannelSelectionManager : MonoBehaviour
{
    public TMP_Text channelName;
    public TMP_Text componentName;
    private string[] channels;
    private int count = 0;
    // Start is called before the first frame update
    void Start()
    {
        channels = ChannelsManager.Instance.GetChannelNames();
        ChannelsManager.Instance.TODOREMOVE_currentChannelSelection = channels[0];
        componentName.text = "TMP";// ChannelsManager.Instance.currentComponent.name;
    }
    public void ChannelToggle()
    {
        count++;
        if (count >= channels.Length)
            count = 0;
        Debug.Log(channels[count]);
        channelName.text = channels[count];
        ChannelsManager.Instance.ChannelSwitch(channels[count]);
        componentName.text = ChannelsManager.Instance.currentComponent.name;
    }

    public void UpdateComponentName(string name)
    {

    }
}
