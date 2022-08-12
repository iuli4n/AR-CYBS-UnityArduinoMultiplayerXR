using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChannelValueInverter : MonoBehaviour
{
    public AtomicDataModel sourceChannel;
    public AtomicDataModel destChannel;

    void Start()
    {
        
    }

    void Update()
    {
        destChannel.Value = 1024 - sourceChannel.Value;
    }
}
