using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChannelSelectionToggle : MonoBehaviour
{
    public void ChannelSelection()
    {
        ChannelsManager.Instance.currentComponent = this.gameObject;
    }
}
