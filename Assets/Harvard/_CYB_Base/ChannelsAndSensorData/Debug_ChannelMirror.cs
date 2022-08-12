using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_ChannelMirror : MonoBehaviour
{
    public string fromChannel;
    public string toChannel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ChannelsManager.Instance.GetModelForChannel(toChannel).Value = ChannelsManager.Instance.GetModelForChannel(fromChannel).Value;
    }
}
