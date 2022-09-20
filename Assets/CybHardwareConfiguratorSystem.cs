using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CybHardwareConfiguratorSystem : MonoBehaviour
{
    // This class reads the configuration settings from CybHardwareProfileConfiguration.Instance
    // and configures all the individual project components to work with the specified settings.

    // links to the objects we will configure

    public GameObject objArduino_SerialPort;
    public SerialController objArduino_SerialController;
    public SerialManager objArduino_SerialManager;

    public GameObject simchannelsObj;

    private void Awake()
    {

#if UNITY_EDITOR
        if (CybHardwareProfileConfiguration.Instance.arduino_Enabled)
        {
            objArduino_SerialController.portName = CybHardwareProfileConfiguration.Instance.arduino_ComPort;

            objArduino_SerialManager.channelsFromArduino = CybHardwareProfileConfiguration.Instance.arduino_channelsFromArduino;
            objArduino_SerialManager.channelsToArduino = CybHardwareProfileConfiguration.Instance.arduino_channelsToArduino;

            objArduino_SerialPort.SetActive(true);
        }
        else
        {
            objArduino_SerialPort.SetActive(false);
        }

        if (CybHardwareProfileConfiguration.Instance.simchannels_Enabled)
        {
            if (CybHardwareProfileConfiguration.Instance.simchannels_spikyChannel.Length > 0)
                simchannelsObj.GetComponent<ElectronicsDataPipeSender>().sensorModel1 = ChannelsManager.Instance.GetModelForChannel(CybHardwareProfileConfiguration.Instance.simchannels_spikyChannel);
            else
                simchannelsObj.GetComponent<ElectronicsDataPipeSender>().sensorModel1 = null;

            if (CybHardwareProfileConfiguration.Instance.simchannels_sineChannel.Length > 0)
                simchannelsObj.GetComponent<ElectronicsDataPipeSender>().sensorModel2 = ChannelsManager.Instance.GetModelForChannel(CybHardwareProfileConfiguration.Instance.simchannels_sineChannel);
            else
                simchannelsObj.GetComponent<ElectronicsDataPipeSender>().sensorModel2 = null;

            if (CybHardwareProfileConfiguration.Instance.simchannels_squareChannel.Length > 0)
                simchannelsObj.GetComponent<ElectronicsDataPipeSender>().sensorModel3 = ChannelsManager.Instance.GetModelForChannel(CybHardwareProfileConfiguration.Instance.simchannels_squareChannel);
            else
                simchannelsObj.GetComponent<ElectronicsDataPipeSender>().sensorModel3 = null;

            if (CybHardwareProfileConfiguration.Instance.simchannels_increasingChannel.Length > 0)
                simchannelsObj.GetComponent<ElectronicsDataPipeSender>().sensorModel4 = ChannelsManager.Instance.GetModelForChannel(CybHardwareProfileConfiguration.Instance.simchannels_increasingChannel);
            else
                simchannelsObj.GetComponent<ElectronicsDataPipeSender>().sensorModel4 = null;

            simchannelsObj.SetActive(true);
        }
        else
        {
            simchannelsObj.SetActive(false);
        }
#else

    Debug.LogWarning("Hardware configuration system: This platform is not the unity editor, so not performing any non-editor configuration.");

#endif


    }
}
