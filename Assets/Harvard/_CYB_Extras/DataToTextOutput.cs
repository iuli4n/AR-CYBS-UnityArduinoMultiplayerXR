using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DataToTextOutput : MonoBehaviour
{
    public AtomicDataSwitch dataSwitch;
    public TextMeshPro text;

    // Start is called before the first frame update
    void Start()
    {
        dataSwitch.OnDataUpdated += OnDataUpdated;
        text.text = "Channel " + dataSwitch.CurrentChannel + "\n Value uninitialized";
    }

    void OnDataUpdated(float newValue)
    {
        text.text = "Channel "+dataSwitch.CurrentChannel+"\n Value "+ newValue.ToString("0.00");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
