using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CybHardwareProfileConfiguration : MonoBehaviour
{
    // This class enables/configures gameobjects according to what configuration the user wants. 
    // The configuration happens only on Awake, and is put into existing objects, so any configuration there may be overwritten.


    public static CybHardwareProfileConfiguration Instance;

    // arduino connection
    public bool arduino_Enabled;
    public string arduino_ComPort;  // example pc: COM8   mac: /dev/tty0
    public string[] arduino_channelsFromArduino; // list of channels to be received by Unity from the Arduino
    public string[] arduino_channelsToArduino; // list of channels to be sent from Unity to the Arduino

    // simulated channels generator
    public bool simchannels_Enabled;
    public string simchannels_spikyChannel; // give channel name or leave empty if you don't want this simulated data
    public string simchannels_sineChannel; // give channel name or leave empty if you don't want this simulated data
    public string simchannels_squareChannel; // give channel name or leave empty if you don't want this simulated data
    public string simchannels_increasingChannel; // give channel name or leave empty if you don't want this simulated data

    private void Awake()
    {
        Debug.Assert(Instance == null, "Multiple instances detected. This is a singleton and only one script should exist ! Overwriting values.");
        Instance = this;
    }
}
