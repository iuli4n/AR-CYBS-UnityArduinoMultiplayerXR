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

    // links to the objects we will configure

    public GameObject objArduino_SerialPort;
    public SerialController objArduino_SerialController;
    public SerialManager objArduino_SerialManager;

    public GameObject simchannelsObj;

    private void Awake()
    {
        Debug.Assert(Instance == null, "Multiple instances detected. This is a singleton and only one script should exist ! Overwriting values.");
        Instance = this;


        if (arduino_Enabled)
        {
            objArduino_SerialController.portName = arduino_ComPort;

            objArduino_SerialManager.channelsFromArduino = arduino_channelsFromArduino;
            objArduino_SerialManager.channelsToArduino = arduino_channelsToArduino;

            objArduino_SerialPort.SetActive(true);
        } else
        {
            objArduino_SerialPort.SetActive(false);
        }


        if (simchannels_Enabled)
        {
            if (simchannels_spikyChannel.Length > 0)
                simchannelsObj.GetComponent<ElectronicsDataPipeSender>().sensorModel1 = ChannelsManager.Instance.GetModelForChannel(simchannels_spikyChannel);
            else
                simchannelsObj.GetComponent<ElectronicsDataPipeSender>().sensorModel1 = null;

            if (simchannels_sineChannel.Length > 0)
                simchannelsObj.GetComponent<ElectronicsDataPipeSender>().sensorModel2 = ChannelsManager.Instance.GetModelForChannel(simchannels_sineChannel);
            else
                simchannelsObj.GetComponent<ElectronicsDataPipeSender>().sensorModel2 = null;

            if (simchannels_squareChannel.Length > 0)
                simchannelsObj.GetComponent<ElectronicsDataPipeSender>().sensorModel3 = ChannelsManager.Instance.GetModelForChannel(simchannels_squareChannel);
            else
                simchannelsObj.GetComponent<ElectronicsDataPipeSender>().sensorModel3 = null;

            if (simchannels_increasingChannel.Length > 0)
                simchannelsObj.GetComponent<ElectronicsDataPipeSender>().sensorModel4 = ChannelsManager.Instance.GetModelForChannel(simchannels_increasingChannel);
            else
                simchannelsObj.GetComponent<ElectronicsDataPipeSender>().sensorModel4 = null;

            simchannelsObj.SetActive(true);
        } else
        {
            simchannelsObj.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
